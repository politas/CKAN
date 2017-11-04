using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CKAN;
using CKAN.Versioning;
using Version = CKAN.Version;

namespace Tests.Data
{
    static public class TestData
    {
        public static string DataDir()
        {
            // FIXME: Come up with a better solution for test data
            // 1. This is fragile with respect to changes in directory structure.
            // 2. This forces us to disable ReSharper's test assembly shadow copying
            // 3. "Quick" solution is to embed test data as an archive and extract it on demand to a known temporary location.
            //    But this makes updates hard.
            // 4. A better, but much harder solution, is to not require harded files on disk for any of our tests, but that's
            //    a lot of work.
            return Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "../../../../../Tests/Data");
        }

        public static string DataDir(string file)
        {
            return Path.Combine(DataDir(), file);
        }

        /// <summary>
        /// Returns the full path to DogeCoinFlag-1.01.zip
        /// </summary>
        public static string DogeCoinFlagZip()
        {
            string such_zip_very_currency_wow = Path.Combine(DataDir(), "DogeCoinFlag-1.01.zip");

            return such_zip_very_currency_wow;
        }

        public static string DogeCoinFlagAvcZip()
        {
            return Path.Combine(DataDir(), "DogeCoinFlag-1.01-avc.zip");
        }

        /// <summary>
        /// Returns DogeCoinFlag.zip, with extra files inside.
        /// Great for testing filters.
        /// </summary>
        public static string DogeCoinFlagZipWithExtras()
        {
            return Path.Combine(DataDir(), "DogeCoinFlag-extra-files.zip");
        }

        /// <summary>
        /// Returns the full path to DogeCoinFlag-1.01-corrupt.zip
        /// </summary>
        public static string DogeCoinFlagZipCorrupt()
        {
            string such_zip_very_corrupt_wow = Path.Combine(DataDir(), "DogeCoinFlag-1.01-corrupt.zip");

            return such_zip_very_corrupt_wow;
        }

        /// <summary>
        /// Adds a plugins directory to the DogeCoinFlag directory to test.
        /// </summary>
        /// <returns>The coin plugin.</returns>
        public static string DogeCoinPluginZip()
        {
            return Path.Combine(DataDir(), "DogeCoinPlugin.zip");
        }


        ///<summary>
        /// DogeCoinFlag 1.01 info. This contains a bug where the
        /// install stanza doesn't actually refer to any files.
        ///</summary>
        public static string DogeCoinFlag_101_bugged()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""identifier"": ""DogeCoinFlag"",
                    ""install"": [
                        {
                        ""file"": ""GameData/DogeCoinFlag"",
                        ""install_to"": ""GameData""
                        }
                    ],
                    ""resources"": {
                        ""kerbalstuff"": {
                        ""url"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag""
                        },
                        ""homepage"": ""https://www.reddit.com/r/dogecoin/comments/1tdlgg/i_made_a_more_accurate_dogecoin_and_a_ksp_flag/""
                    },
                    ""name"": ""Dogecoin Flag"",
                    ""license"": ""CC-BY"",
                    ""abstract"": ""Such flag. Very currency. To the mun! Wow!"",
                    ""author"": ""pjf"",
                    ""version"": ""1.01"",
                    ""download"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag/download/1.01"",
                    ""comment"": ""Generated by ks2ckan"",
                    ""download_size"": 53647,
                    ""ksp_version"": ""0.25""
                }
            ";
        }

        public static CkanModule DogeCoinFlag_101_bugged_module()
        {
            return CkanModule.FromJson(DogeCoinFlag_101_bugged());
        }

        ///<summary>
        /// DogeCoinFlag 1.01 info. This doesn't contain any bugs.
        ///</summary>
        public static string DogeCoinFlag_101()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""identifier"": ""DogeCoinFlag"",
                    ""install"": [
                        {
                        ""file"": ""DogeCoinFlag-1.01/GameData/DogeCoinFlag"",
                        ""install_to"": ""GameData"",
                        ""filter"" : [ ""Thumbs.db"", ""README.md"" ],
                        ""filter_regexp"" : ""\\.bak$""
                        }
                    ],
                    ""resources"": {
                        ""kerbalstuff"": {
                        ""url"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag""
                        },
                        ""homepage"": ""https://www.reddit.com/r/dogecoin/comments/1tdlgg/i_made_a_more_accurate_dogecoin_and_a_ksp_flag/""
                    },
                    ""name"": ""Dogecoin Flag"",
                    ""license"": ""CC-BY"",
                    ""abstract"": ""Such flag. Very currency. To the mun! Wow!"",
                    ""author"": ""pjf"",
                    ""version"": ""1.01"",
                    ""download"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag/download/1.01"",
                    ""comment"": ""Generated by ks2ckan"",
                    ""download_size"": 53647,
                    ""ksp_version"": ""0.25""
                }
            ";
        }

        ///<summary>
        /// Replaced_by DogeCoinFlag 1.01 info. This doesn't contain any bugs.
        ///</summary>
        public static string DogeCoinFlag_101_replaced()
        {
            return @"
                {
                    ""spec_version"": ""v1.24"",
                    ""identifier"": ""DogeCoinFlag"",
                    ""install"": [
                        {
                        ""file"": ""DogeCoinFlag-1.01/GameData/DogeCoinFlag"",
                        ""install_to"": ""GameData"",
                        ""filter"" : [ ""Thumbs.db"", ""README.md"" ],
                        ""filter_regexp"" : ""\\.bak$""
                        }
                    ],
                    ""replaced_by"": {
                        ""name"": ""DogeTokenFlag"",
                        ""min_version"": ""1.01""
                    ),
                    ""resources"": {
                        ""kerbalstuff"": {
                        ""url"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag""
                        },
                        ""homepage"": ""https://www.reddit.com/r/dogecoin/comments/1tdlgg/i_made_a_more_accurate_dogecoin_and_a_ksp_flag/""
                    },
                    ""name"": ""Dogecoin Flag"",
                    ""license"": ""CC-BY"",
                    ""abstract"": ""Such flag. Very currency. To the mun! Wow!"",
                    ""author"": ""pjf"",
                    ""version"": ""1.01"",
                    ""download"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag/download/1.01"",
                    ""comment"": ""Generated by ks2ckan"",
                    ""download_size"": 53647,
                    ""ksp_version"": ""0.24""
                }
            ";
        }

        ///<summary>
        /// DogeTokenFlag 1.01 info. This is our replacement target.
        ///</summary>
        public static string DogeTokenFlag_101()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""identifier"": ""DogeTokenFlag"",
                    ""install"": [
                        {
                        ""file"": ""DogeTokenFlag-1.01/GameData/DogeTokenFlag"",
                        ""install_to"": ""GameData"",
                        ""filter"" : [ ""Thumbs.db"", ""README.md"" ],
                        ""filter_regexp"" : ""\\.bak$""
                        }
                    ],
                    ""resources"": {
                        ""homepage"": ""https://www.reddit.com/r/dogecoin/comments/1tdlgg/i_made_a_more_accurate_dogecoin_and_a_ksp_flag/""
                    },
                    ""name"": ""Dogetoken Flag"",
                    ""license"": ""CC-BY"",
                    ""abstract"": ""Such flag. Very token. To the mun! Wow!"",
                    ""author"": ""politas"",
                    ""version"": ""1.01"",
                    ""download"": ""https://kerbalstuff.com/mod/269/Dogetoken%20Flag/download/1.01"",
                    ""comment"": ""Generated by hand"",
                    ""download_size"": 53647,
                    ""ksp_version"": ""0.25""
                }
            ";
        }

        public static CkanModule DogeCoinFlag_101_module()
        {
            return CkanModule.FromJson(DogeCoinFlag_101());
        }

        /// <summary>
        /// The Doge Coin flag we all love, but using `find` install stanzas.
        /// </summary>
        public static CkanModule DogeCoinFlag_101_module_find()
        {
            CkanModule doge = DogeCoinFlag_101_module();

            // Hand hack in the 'find' directive.
            doge.install[0].file = null;
            doge.install[0].find = "DogeCoinFlag";

            return doge;
        }

        /// <summary>
        /// The Doge Coin flag using include_only and include_only_regexp.
        /// Won't install the same files as the other modules and the files don't make sense but I needed some module to test the include_only stuff
        /// </summary>
        public static CkanModule DogeCoinFlag_101_module_include()
        {
            CkanModule doge = DogeCoinFlag_101_module();

            doge.install[0].filter = null;
            doge.install[0].filter_regexp = null;
            doge.install[0].include_only = new List<String> { "dogecoin.png" };
            doge.install[0].include_only_regexp = new List<string> { "\\.bak$" };

            return doge;
        }

        // Identical to DogeCoinFlag_101, but with a spec version over 9000!
        public static string FutureMetaData()
        {
            return @"
                {
                    ""spec_version"": ""v9000.0.1"",
                    ""identifier"": ""DogeCoinFlag"",
                    ""install"": [
                        {
                        ""file"": ""DogeCoinFlag-1.01/GameData/DogeCoinFlag"",
                        ""install_to"": ""GameData"",
                        ""filter"" : [ ""Thumbs.db"", ""README.md"" ],
                        ""filter_regexp"" : ""\\.bak$""
                        }
                    ],
                    ""resources"": {
                        ""kerbalstuff"": {
                        ""url"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag""
                        },
                        ""homepage"": ""https://www.reddit.com/r/dogecoin/comments/1tdlgg/i_made_a_more_accurate_dogecoin_and_a_ksp_flag/""
                    },
                    ""name"": ""Dogecoin Flag"",
                    ""license"": ""CC-BY"",
                    ""abstract"": ""Such flag. Very currency. To the mun! Wow!"",
                    ""author"": ""pjf"",
                    ""version"": ""1.01"",
                    ""download"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag/download/1.01"",
                    ""comment"": ""Generated by ks2ckan"",
                    ""download_size"": 53647,
                    ""ksp_version"": ""0.25""
                }
            ";
        }

        ///<summary>
        /// DogeCoinPlugin info. This doesn't contain any bugs.
        ///</summary>
        public static string DogeCoinPlugin()
        {
            return @"
                {
                    ""spec_version"": ""v1.2"",
                    ""identifier"": ""DogeCoinPlugin"",
                    ""install"": [
                        {
                        ""file"": ""GameData/DogeCoinFlag/plugin"",
                        ""install_to"": ""GameData/DogeCoinFlag"",
                        ""filter"" : [ ""Thumbs.db"", ""README.md"" ],
                        ""filter_regexp"" : ""\\.bak$""
                        }
                    ],
                    ""resources"": {
                        ""kerbalstuff"": {
                        ""url"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag""
                        },
                        ""homepage"": ""https://www.reddit.com/r/dogecoin/comments/1tdlgg/i_made_a_more_accurate_dogecoin_and_a_ksp_flag/""
                    },
                    ""name"": ""Dogecoin Core Plugin"",
                    ""license"": ""CC-BY"",
                    ""abstract"": ""Such plugin. Very linkage. Dynamically Minmus! Wow!"",
                    ""author"": ""politas"",
                    ""version"": ""1.01"",
                    ""download"": ""https://kerbalstuff.com/mod/269/Dogecoin%20Flag/download/1.01"",
                    ""download_size"": 53647,
                    ""ksp_version"": ""0.25""
                }
            ";
        }

        public static CkanModule DogeCoinPlugin_module()
        {
            return CkanModule.FromJson(DogeCoinPlugin());
        }

        /// <summary>
        /// Taurus HCV pod, which seems to cause weird KS errors when the unescaped
        /// download string is used.
        /// </summary>
        public static string RandSCapsuleDyne()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""name"": ""Taurus HCV - 3.75 m Stock-ish Crew Pod"",
                    ""identifier"": ""RandSCapsuledyne"",
                    ""license"": ""CC-BY-SA-3.0"",
                    ""install"": [
                        {
                            ""file"": ""GameData/R&SCapsuledyne"",
                            ""install_to"": ""GameData""
                        }
                    ],
                    ""depends"": [
                        {
                            ""name"": ""BDAnimationModules""
                        }
                    ],
                    ""resources"": {
                        ""homepage"": ""http://forum.kerbalspaceprogram.com/threads/75074-Taurus-HCV-3-75-m-Stock-ish-Crew-Pod-v-b0-5-April-4-2014?p=1064792#post1064792"",
                        ""kerbalstuff"": ""https://kerbalstuff.com/mod/13/Taurus%20HCV%20-%203.75%20m%20Stock-ish%20Crew%20Pod""
                    },
                    ""ksp_version"": ""0.90"",
                    ""abstract"": ""0.90.0 COMPATIBLE! The Taurus High Capacity Vehicle is a 7 kerbal, 3.75-m cockpit designed to integrate well with the stock game. "",
                    ""author"": ""jnrobinson"",
                    ""version"": ""1.4.0"",
                    ""download"": ""https://kerbalstuff.com/mod/13/Taurus%20HCV%20-%203.75%20m%20Stock-ish%20Crew%20Pod/download/1.4.0"",
                    ""x_generated_by"": ""netkan"",
                    ""download_size"": 8351916
                }
            ";
        }

        public static CkanModule RandSCapsuleDyneModule()
        {
            return CkanModule.FromJson(RandSCapsuleDyne());
        }

        // TestKAN in tar.gz format.
        public static Uri TestKANTarGz()
        {
            return new Uri(DataDir("CKAN-meta-testkan.tar.gz"), UriKind.Relative);
        }

        // TestKAN in zip format.
        public static Uri TestKANZip()
        {
            return new Uri(DataDir("CKAN-meta-testkan.zip"), UriKind.Relative);
        }

        // A repo full of deliciously bad metadata in tar.gz format.
        public static Uri BadKANTarGz()
        {
            return new Uri(DataDir("CKAN-meta-badkan.tar.gz"));
        }

        // A repo full of deliciously bad metadata in zip format.
        public static Uri BadKANZip()
        {
            return new Uri(DataDir("CKAN-meta-badkan.zip"));
        }

        public static string good_ksp_dir()
        {
            return Path.Combine(DataDir(), "KSP/KSP-0.25");
        }

        public static IEnumerable<string> bad_ksp_dirs()
        {
            var dirs = new List<string>
            {
                Path.Combine(DataDir(), "KSP/bad-ksp"),
                Path.Combine(DataDir(), "KSP/missing-gamedata")
            };

            return dirs;
        }

        public static string kOS_014()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""name""     : ""kOS - Kerbal OS"",
                    ""identifier"" : ""kOS"",
                    ""abstract"" : ""A programming and automation environment for KSP craft."",
                    ""download"" : ""https://github.com/KSP-KOS/KOS/releases/download/v0.14/kOS.v14.zip"",
                    ""license""  : ""GPL-3.0"",
                    ""version""  : ""0.14"",
                    ""release_status"" : ""stable"",
                    ""ksp_version"" : ""0.24.2"",
                    ""resources"" : {
                        ""homepage"" : ""http://forum.kerbalspaceprogram.com/threads/68089-0-23-kOS-Scriptable-Autopilot-System-v0-11-2-13"",
                        ""manual""   : ""http://ksp-kos.github.io/KOS_DOC/"",
                        ""bugtracker"": ""https://github.com/KSP-KOS/KOS/issues"",
                        ""repository""   : ""https://github.com/KSP-KOS/KOS""
                    },
                    ""install"" : [
                        {
                            ""file""       : ""GameData/kOS"",
                            ""install_to"" : ""GameData""
                        }
                    ],
                    ""download_hash"": {
                        ""sha1"": ""C5A224AC4397770C0B19B4A6417F6C5052191608"",
                        ""sha256"": ""E0FB79C81D8FCDA8DB6E38B104106C3B7D078FDC06ACA2BC7834973B43D789CB""
                    }
                }"
            ;
        }

        public static string kOS_014_with_invalid_version_characters()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""name""     : ""kOS - Kerbal OS"",
                    ""identifier"" : ""kOS"",
                    ""abstract"" : ""A programming and automation environment for KSP craft."",
                    ""download"" : ""https://github.com/KSP-KOS/KOS/releases/download/v0.14/kOS.v14.zip"",
                    ""license""  : ""GPL-3.0"",
                    ""version""  : ""0:14^0"",
                    ""release_status"" : ""stable"",
                    ""ksp_version"" : ""0.24.2"",
                    ""resources"" : {
                        ""homepage"" : ""http://forum.kerbalspaceprogram.com/threads/68089-0-23-kOS-Scriptable-Autopilot-System-v0-11-2-13"",
                        ""manual""   : ""http://ksp-kos.github.io/KOS_DOC/"",
                        ""bugtracker"": ""https://github.com/KSP-KOS/KOS/issues"",
                        ""github""   : {
                            ""url""      : ""https://github.com/KSP-KOS/KOS"",
                            ""releases"" : true
                        }
                    },
                    ""install"" : [
                        {
                            ""file""       : ""GameData/kOS"",
                            ""install_to"" : ""GameData""
                        }
                    ]
                }"
            ;
        }

        // AFAIK kOS isn't multi-licensed, but we need somthing for testing. :)
        public static string kOS_014_multilicense()
        {
            return @"
                {
                    ""spec_version"": 1,
                    ""name""     : ""kOS - Kerbal OS"",
                    ""identifier"" : ""kOS"",
                    ""abstract"" : ""A programming and automation environment for KSP craft."",
                    ""download"" : ""https://github.com/KSP-KOS/KOS/releases/download/v0.14/kOS.v14.zip"",
                    ""license""  : [ ""GPL-3.0"", ""GPL-2.0"" ],
                    ""version""  : ""0.14"",
                    ""release_status"" : ""stable"",
                    ""ksp_version"" : ""0.24.2"",
                    ""resources"" : {
                        ""homepage"" : ""http://forum.kerbalspaceprogram.com/threads/68089-0-23-kOS-Scriptable-Autopilot-System-v0-11-2-13"",
                        ""manual""   : ""http://ksp-kos.github.io/KOS_DOC/"",
                        ""bugtracker"": ""https://github.com/KSP-KOS/KOS/issues"",
                        ""github""   : {
                            ""url""      : ""https://github.com/KSP-KOS/KOS"",
                            ""releases"" : true
                        }
                    },
                    ""install"" : [
                        {
                            ""file""       : ""GameData/kOS"",
                            ""install_to"" : ""GameData""
                        }
                    ]
                }"
            ;
        }

        public static CkanModule kOS_014_module()
        {
            return CkanModule.FromJson(kOS_014());
        }

        public static string KS_CustomAsteroids_string()
        {
            return File.ReadAllText(Path.Combine(DataDir(), "KS/CustomAsteroids.json"));
        }

        public static CkanModule FireSpitterModule()
        {
            return CkanModule.FromFile(Path.Combine(DataDir(), "Firespitter-6.3.5.ckan"));
        }

        public static string KspAvcJson()
        {
            return File.ReadAllText(Path.Combine(DataDir(), "ksp-avc.version"));
        }


        public static string KspAvcJsonOneLineVersion()
        {
            return File.ReadAllText(Path.Combine(DataDir(), "ksp-avc-one-line.version"));
        }

        public static CkanModule ModuleManagerModule()
        {
            return CkanModule.FromFile(DataDir("ModuleManager-2.5.1.ckan"));
        }

        public static string ModuleManagerZip()
        {
            return DataDir("ModuleManager-2.5.1.zip");
        }

        /// <summary>
        /// A path to our test registry.json file. Please copy before using.
        /// </summary>
        public static string TestRegistry()
        {
            return DataDir("registry.json");
        }

        // Where's my mkdtemp? Instead we'll make a random file, delete it, and
        // fill its place with a directory.
        // Taken from https://stackoverflow.com/a/20445952
        public static string NewTempDir()
        {
            string temp_folder = Path.GetTempFileName();
            File.Delete(temp_folder);
            Directory.CreateDirectory(temp_folder);

            return temp_folder;
        }

        // Ugh, this is awful.
        public static void CopyDirectory(string src, string dst)
        {
            // Create directory structure
            foreach (string path in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(path.Replace(src, dst));
            }

            // Copy files.
            foreach (string file in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(src, dst));
            }
        }

        public static string ConfigurationFile()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
            <Configuration xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
              <CommandLineArguments>KSP.exe -force-opengl</CommandLineArguments>
              <AutoCloseWaitDialog>false</AutoCloseWaitDialog>
              <URLHandlerNoNag>false</URLHandlerNoNag>
              <CheckForUpdatesOnLaunch>true</CheckForUpdatesOnLaunch>
              <CheckForUpdatesOnLaunchNoNag>true</CheckForUpdatesOnLaunchNoNag>
              <HideEpochs>true</HideEpochs>
              <SortByColumnIndex>2</SortByColumnIndex>
              <SortDescending>false</SortDescending>
              <WindowSize>
                <Width>1024</Width>
                <Height>664</Height>
              </WindowSize>
              <WindowLoc>
                <X>512</X>
                <Y>136</Y>
              </WindowLoc>
            </Configuration>";
        }
    }

    public class RandomModuleGenerator
    {
        public Random Generator { get; set; }

        public RandomModuleGenerator(Random generator)
        {
            Generator = generator;
        }

        public CkanModule GeneratorRandomModule(
            KspVersion ksp_version = null,
            List<RelationshipDescriptor> conflicts = null,
            List<RelationshipDescriptor> depends = null,
            List<RelationshipDescriptor> sugests = null,
            List<String> provides = null,
            string identifier = null,
            Version version = null)
        {
            var mod = new CkanModule
            {
                name = Generator.Next().ToString(CultureInfo.InvariantCulture),
                @abstract = Generator.Next().ToString(CultureInfo.InvariantCulture),
                identifier = identifier ?? Generator.Next().ToString(CultureInfo.InvariantCulture),
                spec_version = new Version(1.ToString(CultureInfo.InvariantCulture)),
                ksp_version = ksp_version ?? KspVersion.Parse("0." + Generator.Next()),
                version = version ?? new Version(Generator.Next().ToString(CultureInfo.InvariantCulture))
            };
            mod.ksp_version_max = mod.ksp_version_min = null;
            mod.conflicts = conflicts;
            mod.depends = depends;
            mod.suggests = sugests;
            mod.provides = provides;
            return mod;
        }
    }
}

