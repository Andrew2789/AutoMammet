using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
using AutoMammet.Windows;
using System.IO;

namespace AutoMammet
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "AutoMammetFix";
        private const string CommandName = "/mammet";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("AutoMammetFix");

        public Window window;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            string valueMappingPath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "AutoMammetValueMapping.json");

            Reader reader = new Reader(this.PluginInterface, this);
            window = new MainWindow(this, reader, valueMappingPath);
            WindowSystem.AddWindow(window);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open up the exporting UI/Interface."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;

        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            window.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }
    }
}
