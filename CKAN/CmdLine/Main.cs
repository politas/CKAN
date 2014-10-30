// Reference CKAN client
// Paul '@pjf' Fenwick
//
// License: CC-BY 4.0, LGPL, or MIT (your choice)

using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using log4net;
using log4net.Config;
using log4net.Core;
using System.Transactions;

namespace CKAN.CmdLine
{
    internal class MainClass
    {
        public const int EXIT_OK = 0;
        public const int EXIT_ERROR = 1;
        public const int EXIT_BADOPT = 2;

        private static readonly ILog log = LogManager.GetLogger(typeof (MainClass));

        public static int Main(string[] args)
        {
            BasicConfigurator.Configure();
            LogManager.GetRepository().Threshold = Level.Warn;
            log.Debug("CKAN started");

            // If we're starting with no options, invoke the GUI instead.

            if (args.Length == 0)
            {
                return Gui();
            }

            Options cmdline;

            try
            {
                cmdline = new Options(args);
            }
            catch (NullReferenceException)
            {
                // Oops, something went wrong. Generate the help screen instead!

                string[] help = {"--help"}; // Is there a nicer way than a temp var?
                new Options(help);
                return EXIT_BADOPT;
            }

            // Process commandline options.

            var options = (CommonOptions) cmdline.options;

            if (options.Debug)
            {
                LogManager.GetRepository().Threshold = Level.Debug;
            }
            else if (options.Verbose)
            {
                LogManager.GetRepository().Threshold = Level.Info;
            }

            // TODO: Allow the user to specify just a directory.
            // User provided KSP instance

            if (options.KSPdir != null && options.KSP != null)
            {
                User.WriteLine("--ksp and --kspdir can't be specified at the same time");
                return EXIT_BADOPT;
            }

            if (options.KSP != null)
            {
                // Set a KSP directory by its alias.

                try
                {
                    KSPManager.SetCurrentInstance(options.KSP);
                }
                catch (InvalidKSPInstanceKraken)
                {
                    User.WriteLine("Invalid KSP installation specified \"{0}\", use '--kspdir' to specify by path, or 'list-installs' to see known KSP installations", options.KSP);
                    return EXIT_BADOPT;
                }
            }
            else if (options.KSPdir != null)
            {
                // Set a KSP directory by its path

                KSPManager.SetCurrentInstanceByPath(options.KSPdir);

            }

            else
            {
                // auto-start instance
                KSPManager.GetPreferredInstance();
            }

            User.WriteLine("Using KSP installation at \"{0}\"", KSPManager.CurrentInstance.GameDir());

            switch (cmdline.action)
            {
                case "gui":
                    return Gui();

                case "version":
                    return Version();

                case "update":
                    return Update((UpdateOptions) options);

                case "available":
                    return Available();

                case "install":
                    return Install((InstallOptions) cmdline.options);

                case "scan":
                    return Scan();

                case "list":
                    return List();

                case "show":
                    return Show((ShowOptions) cmdline.options);

                case "remove":
                    return Remove((RemoveOptions) cmdline.options);

                case "upgrade":
                    return Upgrade((UpgradeOptions) cmdline.options);

                case "clean":
                    return Clean();

                // TODO: Combine all of these into `ckan kspdir ...` or something similar
                // Eg: `ckan kspdir add` `ckan kspdir list` etc

                case "list-installs":
                    return ListInstalls();

                case "add-install":
                    return AddInstall((AddInstallOptions) cmdline.options);

                case "rename-install":
                    return RenameInstall((RenameInstallOptions)cmdline.options);

                case "remove-install":
                    return RemoveInstall((RemoveInstallOptions)cmdline.options);

                case "set-default-install":
                    return SetDefaultInstall((SetDefaultInstallOptions)cmdline.options);

                case "clear-cache":
                    return ClearCache((ClearCacheOptions)cmdline.options);

                default:
                    User.WriteLine("Unknown command, try --help");
                    return EXIT_BADOPT;
            }
        }

        private static int Gui()
        {
            // TODO: Sometimes when the GUI exits, we get a System.ArgumentException,
            // but trying to catch it here doesn't seem to help. Dunno why.

            GUI.Main();

            return EXIT_OK;
        }

        private static int Version()
        {
            User.WriteLine(Meta.Version());

            return EXIT_OK;
        }

        private static int Update(UpdateOptions options)
        {
            User.WriteLine("Downloading updates...");

            int updated = Repo.Update(options.repo);

            User.WriteLine("Updated information on {0} available modules", updated);

            return EXIT_OK;
        }

        private static int Available()
        {
            List<CkanModule> available = RegistryManager.Instance(KSPManager.CurrentInstance).registry.Available();

            User.WriteLine("Mods available for KSP {0}", KSPManager.CurrentInstance.Version());
            User.WriteLine("");

            foreach (CkanModule module in available)
            {
                User.WriteLine("* {0}", module);
            }

            return EXIT_OK;
        }

        private static int Scan()
        {
            KSPManager.CurrentInstance.ScanGameData();

            return EXIT_OK;
        }

        private static int List()
        {
            KSP ksp = KSPManager.CurrentInstance;

            User.WriteLine("\nKSP found at {0}\n", ksp.GameDir());
            User.WriteLine("KSP Version: {0}\n",ksp.Version());

            Registry registry = RegistryManager.Instance(ksp).registry;

            User.WriteLine("Installed Modules:\n");

            var installed = new SortedDictionary<string, Version>(registry.Installed());

            foreach (var mod in installed)
            {
                User.WriteLine("* {0} {1}", mod.Key, mod.Value);
            }

            // Blank line at the end makes for nicer looking output.
            User.WriteLine("");

            return EXIT_OK;
        }

        // Uninstalls a module, if it exists.
        private static int Remove(RemoveOptions options)
        {
            var installer = ModuleInstaller.Instance;
            installer.Uninstall(options.Modname, true);

            return EXIT_OK;
        }

        // TODO: This needs work! See GH #160.
        private static int Upgrade(UpgradeOptions options)
        {
            if (options.ckan_file == null)
            {
                // Typical case, install from cached CKAN info.

                if (options.modules.Count == 0)
                {
                    // What? No files specified?
                    User.WriteLine(
                        "Usage: ckan upgrade [--with-suggests] [--with-all-suggests] [--no-recommends] Mod [Mod2, ...]");
                    return EXIT_BADOPT;
                }

                // Do our un-installs and re-installs in a transaction. If something goes wrong,
                // we put the user's data back the way it was. (Both Install and Uninstall support transactions.)
                using (var transaction = new TransactionScope ())
                {
                    var installer = ModuleInstaller.Instance;

                    foreach (string module in options.modules)
                    {
                        installer.Uninstall(module, false);
                    }

                    // Prepare options. Can these all be done in the new() somehow?
                    var install_ops = new RelationshipResolverOptions();
                    install_ops.with_all_suggests = options.with_all_suggests;
                    install_ops.with_suggests = options.with_suggests;
                    install_ops.with_recommends = !options.no_recommends;

                    // Install everything requested. :)
                    try
                    {
                        installer.InstallList(options.modules, install_ops);
                    }
                    catch (ModuleNotFoundKraken ex)
                    {
                        User.WriteLine("Module {0} required, but not listed in index.", ex.module);
                        User.WriteLine("If you're lucky, you can do a `ckan update` and try again.");
                        return EXIT_ERROR;
                    }

                    transaction.Complete();
                }
                User.WriteLine("\nDone!\n");

                return EXIT_OK;
            }

            User.WriteLine("\nUnsupported option at this time.");

            return EXIT_BADOPT;
        }

        private static int Clean()
        {
            KSPManager.CurrentInstance.CleanCache();
            return EXIT_OK;
        }

        private static int Install(InstallOptions options)
        {
            if (options.ckan_file != null)
            {
                // Oooh! We're installing from a CKAN file.
                log.InfoFormat("Installing from CKAN file {0}", options.ckan_file);

                CkanModule module = CkanModule.FromFile(options.ckan_file);

                // We'll need to make some registry changes to do this.
                RegistryManager registry_manager = RegistryManager.Instance(KSPManager.CurrentInstance);

                // Remove this version of the module in the registry, if it exists.
                registry_manager.registry.RemoveAvailable(module);

                // Sneakily add our version in...
                registry_manager.registry.AddAvailable(module);

                // Add our module to the things we should install...
                options.modules.Add(module.identifier);

                // And continue with our install as per normal.
            }
 
            if (options.modules.Count == 0)
            {
                // What? No files specified?
                User.WriteLine(
                    "Usage: ckan install [--with-suggests] [--with-all-suggests] [--no-recommends] Mod [Mod2, ...]");
                return EXIT_BADOPT;
            }

            // Prepare options. Can these all be done in the new() somehow?
            var install_ops = new RelationshipResolverOptions();
            install_ops.with_all_suggests = options.with_all_suggests;
            install_ops.with_suggests = options.with_suggests;
            install_ops.with_recommends = ! options.no_recommends;

            // Install everything requested. :)
            try
            {
                var installer = ModuleInstaller.Instance;
                installer.InstallList(options.modules, install_ops);
            }
            catch (ModuleNotFoundKraken ex)
            {
                User.WriteLine("Module {0} required, but not listed in index, or not available for your version of KSP", ex.module);
                User.WriteLine("If you're lucky, you can do a `ckan update` and try again.");
                User.WriteLine("Try `ckan install --no-recommends` to skip installation of recommended modules");
                return EXIT_ERROR;
            }
            catch (BadMetadataKraken ex)
            {
                User.WriteLine("Bad metadata detected for module {0}", ex.module);
                User.WriteLine(ex.Message);
                return EXIT_ERROR;
            }
            catch (TooManyModsProvideKraken ex)
            {
                User.WriteLine("Too many mods provide {0}. Please pick from the following:\n", ex.requested);

                foreach (CkanModule mod in ex.modules)
                {
                    User.WriteLine("* {0}", mod.identifier);
                }

                return EXIT_ERROR;
            }

            User.WriteLine("\nDone!\n");

            return EXIT_OK;
        }

        // TODO: We should have a command (probably this one) that shows
        // info about uninstalled modules.
        private static int Show(ShowOptions options)
        {
            if (options.Modname == null)
            {
                // empty argument
                User.WriteLine("show <module> - module name argument missing, perhaps you forgot it?");
                return EXIT_BADOPT;
            }

            RegistryManager registry_manager = RegistryManager.Instance(KSPManager.CurrentInstance);
            InstalledModule module;

            try
            {
                module = registry_manager.registry.installed_modules[options.Modname];
            }
            catch (KeyNotFoundException)
            {
                User.WriteLine("{0} not installed.", options.Modname);
                User.WriteLine("Try `ckan list` to show installed modules");
                return EXIT_BADOPT;
            }

            // TODO: Print *lots* of information out; I should never have to dig through JSON

            User.WriteLine("{0} version {1}", module.source_module.name, module.source_module.version);

            User.WriteLine("\n== Files ==\n");

            Dictionary<string, InstalledModuleFile> files = module.installed_files;

            foreach (string file in files.Keys)
            {
                User.WriteLine(file);
            }

            return EXIT_OK;
        }

        private static int ListInstalls()
        {
            User.WriteLine("Listing all known KSP installations:");
            User.WriteLine("");

            int count = 1;
            foreach (var instance in KSPManager.Instances)
            {
                User.WriteLine("{0}) \"{1}\" - {2}", count, instance.Key, instance.Value.GameDir());
                count++;
            }

            return EXIT_OK;
        }

        private static int AddInstall(AddInstallOptions options)
        {
            if (options.name == null || options.path == null)
            {
                User.WriteLine("add-install <name> <path> - argument missing, perhaps you forgot it?");
                return EXIT_BADOPT;
            }

            if (KSPManager.Instances.ContainsKey(options.name))
            {
                User.WriteLine("Install with name \"{0}\" already exists, aborting..", options.name);
                return EXIT_BADOPT;
            }

            try
            {

                KSPManager.AddInstance(options.name, options.path);
                User.WriteLine("Added \"{0}\" with root \"{1}\" to known installs", options.name, options.path);
                return EXIT_OK;
            }
            catch (NotKSPDirKraken ex)
            {
                User.WriteLine("Sorry, {0} does not appear to be a KSP directory", ex.path);
                return EXIT_BADOPT;
            }
        }

        private static int RenameInstall(RenameInstallOptions options)
        {
            if (options.old_name == null || options.new_name == null)
            {
                User.WriteLine("rename-install <old_name> <new_name> - argument missing, perhaps you forgot it?");
                return EXIT_BADOPT;
            }

            if (!KSPManager.Instances.ContainsKey(options.old_name))
            {
                User.WriteLine("Couldn't find install with name \"{0}\", aborting..", options.old_name);
                return EXIT_BADOPT;
            }

            KSPManager.RenameInstance(options.old_name, options.new_name);

            User.WriteLine("Successfully renamed \"{0}\" to \"{1}\"", options.old_name, options.new_name);
            return EXIT_OK;
        }

        private static int RemoveInstall(RemoveInstallOptions options)
        {
            if (options.name == null)
            {
                User.WriteLine("remove-install <name> - argument missing, perhaps you forgot it?");
                return EXIT_BADOPT;
            }

            if (!KSPManager.Instances.ContainsKey(options.name))
            {
                User.WriteLine("Couldn't find install with name \"{0}\", aborting..", options.name);
                return EXIT_BADOPT;
            }

            KSPManager.RemoveInstance(options.name);

            User.WriteLine("Successfully removed \"{0}\"", options.name);
            return EXIT_OK;
        }

        private static int SetDefaultInstall(SetDefaultInstallOptions options)
        {
            if (options.name == null)
            {
                User.WriteLine("set-default-install <name> - argument missing, perhaps you forgot it?");
                return EXIT_BADOPT;
            }

            if (!KSPManager.Instances.ContainsKey(options.name))
            {
                User.WriteLine("Couldn't find install with name \"{0}\", aborting..", options.name);
                return EXIT_BADOPT;
            }

            KSPManager.SetAutoStart(options.name);

            User.WriteLine("Successfully set \"{0}\" as the default KSP installation", options.name);
            return EXIT_OK;
        }

        private static int ClearCache(ClearCacheOptions options)
        {
            User.WriteLine("Clearing download cache..");

            var cachePath = Path.Combine(KSPManager.CurrentInstance.CkanDir(), "downloads");
            foreach (var file in Directory.GetFiles(cachePath))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                }
            }

            return EXIT_OK;
        }

    }


    // Exception class, so we can signal errors in command options.

    internal class BadCommandException : Exception
    {
        public BadCommandException(string message) : base(message)
        {
        }
    }
}