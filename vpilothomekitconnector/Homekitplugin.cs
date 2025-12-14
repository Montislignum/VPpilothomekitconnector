using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;
using RossCarlson.Vatsim.Vpilot.Plugins;
using RossCarlson.Vatsim.Vpilot.Plugins.Events;

namespace VPilotHomekitConnector
{
    public class HomekitPlugin : IPlugin
    {
        private const string PLUGIN_NAME = "HomeKit Connector";
        private const int PORT = 3000;

        private IBroker vPilot;
        private string configPath;
        private bool settingsLoaded = false;
        private string raspberryPiIp = "192.168.1.100";
        private string connectedCallsign = null;

        public string Name { get; } = PLUGIN_NAME;

        public void Initialize(IBroker broker)
        {
            vPilot = broker;
            LoadSettings();

            if (settingsLoaded)
            {
                vPilot.NetworkConnected += OnNetworkConnectedHandler;
                vPilot.NetworkDisconnected += OnNetworkDisconnectedHandler;
                vPilot.RadioMessageReceived += OnRadioMessageReceivedHandler;
                vPilot.PrivateMessageReceived += OnPrivateMessageReceivedHandler;

                SendDebug($"HomeKit Connector initialized. Using Raspberry Pi at {raspberryPiIp}");
            }
            else
            {
                SendDebug("HomeKit Connector failed to load. Check your homekit_config.ini");
            }
        }

        private void LoadSettings()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\vPilot");
            if (registryKey != null)
            {
                string vPilotPath = (string)registryKey.GetValue("Install_Dir");
                configPath = Path.Combine(vPilotPath, Path.Combine("Plugins", "homekit_config.ini"));

                if (!File.Exists(configPath))
                {
                    CreateDefaultConfig();
                }

                try
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("RaspberryPiIP="))
                        {
                            raspberryPiIp = line.Substring("RaspberryPiIP=".Length).Trim();
                        }
                    }
                    settingsLoaded = true;
                }
                catch (Exception ex)
                {
                    SendDebug($"Error loading config: {ex.Message}");
                }
            }
            else
            {
                SendDebug("Registry key not found. Is vPilot installed correctly?");
            }
        }

        private void CreateDefaultConfig()
        {
            try
            {
                string defaultConfig = @"; HomeKit Connector Configuration
; Edit this file to set your Raspberry Pi's IP address
RaspberryPiIP=192.168.1.100";

                Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                File.WriteAllText(configPath, defaultConfig);
                SendDebug($"Created default config file at {configPath}");
            }
            catch (Exception ex)
            {
                SendDebug($"Error creating default config: {ex.Message}");
            }
        }

        private void OnNetworkConnectedHandler(object sender, NetworkConnectedEventArgs e)
        {
            connectedCallsign = e.Callsign;
            SendDebug($"Connected with callsign: {connectedCallsign}");
        }

        private void OnNetworkDisconnectedHandler(object sender, EventArgs e)
        {
            connectedCallsign = null;
            SendDebug("Disconnected from network");
        }

        private void OnRadioMessageReceivedHandler(object sender, RadioMessageReceivedEventArgs e)
        {
            if (e.Message.Contains(connectedCallsign))
            {
                NotifyHomebridge("NEW_MESSAGE");
            }
        }

        private void OnPrivateMessageReceivedHandler(object sender, PrivateMessageReceivedEventArgs e)
        {
            NotifyHomebridge("NEW_PRIVATE_MESSAGE");
        }

        private void NotifyHomebridge(string messageType)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(raspberryPiIp, PORT);
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(messageType);
                        stream.Write(data, 0, data.Length);
                    }
                }
                SendDebug($"Notified Homebridge: {messageType}");
            }
            catch (Exception ex)
            {
                SendDebug($"Error notifying Homebridge: {ex.Message} (IP: {raspberryPiIp})");
            }
        }

        private void SendDebug(string text)
        {
            vPilot.PostDebugMessage(text);
        }

        public void Configure()
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", configPath);
            }
            catch (Exception ex)
            {
                SendDebug($"Error opening config file: {ex.Message}");
            }
        }

        public void Shutdown()
        {
            SendDebug("HomeKit Connector shutting down");
        }
    }
}
