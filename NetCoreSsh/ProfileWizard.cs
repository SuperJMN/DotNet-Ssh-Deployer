using System;
using System.Linq;
using System.Reflection;

namespace DotNetSsh
{
    public class ProfileWizard : IProfileWizard
    {
        private readonly Prompt prompt;

        public ProfileWizard()
        {
            prompt = new Prompt();
        }

        public DeploymentProfile Configure(string profileName, ProjectMetadata metadata, string username, DeploymentProfile profile = null)
        {
            if (profile != null)
            {
                UpdateExisting(profileName, profile);
            }
            else
            {
                profile = CreateNew(profileName, username, metadata);
            }

            return profile;
        }

        private DeploymentProfile CreateNew(string profileName, string username, ProjectMetadata metadata)
        {
            Console.WriteLine(Resources.CreatingProfile, profileName);
            Console.WriteLine(Resources.InformationPrompt);
            var settings = new CustomizableSettings();
            var profile = new DeploymentProfile(profileName, settings);

            if (string.IsNullOrWhiteSpace(username))
            {
                username = PromptRemoteUsername();
            }

            FillWithDefaults(username, metadata, settings);
            Fill(settings);
            return profile;
        }

        private void UpdateExisting(string profileName, DeploymentProfile profile)
        {
            Console.WriteLine(Resources.UpdatingProfile, profileName);
            Console.WriteLine(Resources.InformationPrompt);
            Fill(profile.Settings);
        }

        private string PromptRemoteUsername()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Remote username: ");
            Console.ResetColor();
            var name = ReadString();
            Console.WriteLine();
            return name;
        }

        private void FillWithDefaults(string user, ProjectMetadata metadata,
            CustomizableSettings customizableSettings)
        {
            customizableSettings.Host = "hostname";
            customizableSettings.DestinationPath = $"/home/{user ?? "[USER]"}/DotNetApps/{metadata.ProjectName}";
            customizableSettings.Display = ":0.0";
            customizableSettings.RunAfterDeployment = true;
            customizableSettings.AssemblyName = metadata.AssemblyName;
            customizableSettings.Framework = metadata.Frameworks.First();
        }

        private void Fill(object profile)
        {
            foreach (var propertyInfo in profile.GetType().GetProperties())
            {
                Fill(profile, propertyInfo);
            }
        }

        private void Fill(object instance, PropertyInfo property)
        {
            var type = property.PropertyType;

            if (type == typeof(int))
            {
                SetValue(instance, property, () => ReadInt());
            }
            else if (type == typeof(string))
            {
                SetValue(instance, property, () => ReadString());
            }
            else if (type == typeof(bool))
            {
                SetValue(instance, property, () => ReadBool());
            }
            else if (type.IsEnum)
            {
                SetValue(instance, property, () => ReadEnum(type));
            }
            else
            {
                throw new InvalidOperationException($"Unexpected type {type}");
            }
        }

        private void SetValue(object instance, PropertyInfo property, Func<object> getValue)
        {
            var current = property.GetValue(instance);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{property.Name}]");
            Console.ResetColor();
            if (current != default)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Current value: {current}");
                Console.ResetColor();
            }

            var type = property.PropertyType;
            if (type.IsEnum)
            {
                var values = string.Join(", ", Enum.GetNames(type).Select((str, id) => $"{id + 1}. {str}"));
                Console.WriteLine($"(valid values: {values})");
            }

            if (type == typeof(bool))
            {
                Console.WriteLine($"(valid values: {bool.TrueString} / {bool.FalseString})");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("New value: ");
            
            var value = getValue();

            if (value != default)
            {
                property.SetValue(instance, value);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(current);
            }

            Console.ResetColor();

            Console.WriteLine();
        }

        private int ReadInt()
        {
            return Read(s => int.TryParse(s, out var n) ? (n, true) : (default, false));
        }

        private bool? ReadBool()
        {
            return Read(s => bool.TryParse(s, out var n) ? (n, true) : ((bool?)null, false));
        }

        private string ReadString()
        {
            return Read(s => (s, true));
        }

        private Enum ReadEnum(Type enumType)
        {
            return (Enum) Read(s =>
            {
                if (int.TryParse(s, out var option))
                {
                    var values = Enum.GetValues(enumType).Cast<Enum>().ToList();
                    if (option >= 1 && option <= values.Count)
                    {
                        var en = values[option - 1];
                        return (en, true);
                    }

                    return (default, false);
                }


                if (Enum.TryParse(enumType, s, true, out var n))
                {
                    if (Enum.IsDefined(enumType, n))
                    {
                        return (Value: n, true);
                    }
                }

                return (default, false);
            });
        }

        private T Read<T>(Func<string, (T Value, bool Succeeded)> parser)
        {
            var exit = false;
            T n = default;
            do
            {
                var read = prompt.ReadLine(true) ?? "";
                if (read.Trim().Length == 0)
                {
                    return default;
                }

                var parsed = parser(read);
                if (parsed.Succeeded)
                {
                    exit = true;
                    n = parsed.Value;
                }
                else
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("[Error] Invalid entry");
                }
            } while (!exit);

            return n;
        }

        private class Prompt
        {
            private CursorPosition savedPosition;

            public Prompt Write(string prompt)
            {
                Console.Write(prompt);
                return this;
            }

            public Prompt Write(string promptFormat, params object[] args)
            {
                return Write(string.Format(promptFormat, args));
            }

            public Prompt WriteLine(string prompt)
            {
                Write(prompt);
                Console.WriteLine();
                return this;
            }

            public Prompt WriteLine(string promptFormat, params object[] args)
            {
                return WriteLine(string.Format(promptFormat, args));
            }

            public string ReadLine(bool advanceCursorOnSameLine = false, bool eraseLine = false)
            {
                if (advanceCursorOnSameLine || eraseLine)
                {
                    SavePosition();
                    if (eraseLine)
                    {
                        WriteLine(new string(' ', Console.WindowWidth - savedPosition.CursorLeft)).RestorePosition();
                    }
                }

                var input = Console.ReadLine();
                if (advanceCursorOnSameLine)
                {
                    RestorePosition(input.Length);
                }

                return input;
            }

            public Prompt SavePosition()
            {
                savedPosition = GetCursorPosition();
                return this;
            }

            public CursorPosition GetCursorPosition()
            {
                return new CursorPosition
                {
                    CursorLeft = Console.CursorLeft,
                    CursorTop = Console.CursorTop
                };
            }

            public Prompt RestorePosition(CursorPosition position, int deltaLeft = 0, int deltaTop = 0)
            {
                var left = Math.Min(Console.BufferWidth - 1, Math.Max(0, position.CursorLeft + deltaLeft));
                var right = Math.Min(Console.BufferHeight - 1, Math.Max(0, position.CursorTop + deltaTop));
                Console.SetCursorPosition(left, right);
                return this;
            }

            public Prompt RestorePosition(int deltaLeft = 0, int deltaTop = 0)
            {
                return RestorePosition(savedPosition, deltaLeft, deltaTop);
            }

            public struct CursorPosition
            {
                public int CursorLeft;
                public int CursorTop;
            }
        }
    }
}