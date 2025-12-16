\# VPilothomekitconnector



\[!\[GitHub issues](https://img.shields.io/github/issues/montislignum/VPilothomekitconnector.svg)](https://github.com/montislignum/VPilothomekitconnector/issues)

\[!\[GitHub stars](https://img.shields.io/github/stars/montislignum/VPilothomekitconnector.svg)](https://github.com/montislignum/VPilothomekitconnector/stargazers)

\[!\[License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)



A lightweight C# plugin for \[vPilot](https://vpilot.metacraft.com/) that connects to a Homebridge TCP server and triggers HomeKit automations when ATC or private messages are received.



---



\## ✈️ Purpose



This connector bridges vPilot and HomeKit by sending TCP messages to a Raspberry Pi running a Homebridge plugin.  

It enables real-time HomeKit notifications and automations based on vPilot activity.



---



\## ⚙️ Features



\- Reads configuration from a local file (`config.txt`)

\- Sends TCP messages to a configurable IP and port

\- Detects network connection events and message types

\- Logs debug output for troubleshooting



---



\## 🧩 How It Works



```text

vPilot → VPilothomekitconnector → TCP → Raspberry Pi → Homebridge → HomeKit motion sensor → Notification or automation. Use Notify.events to trigger messenger messages.

```



---



\## 📁 Configuration

Paste the dll in the vpilot plugin folder

On first run, the plugin creates a config file:



```ini

\# HomeKit Connector Configuration

Edit this file to set your Raspberry Pi's IP address

RaspberryPiIP=192.168.1.100

```



Update the IP to match your Homebridge device.



---



\## 💡 Example Usage



When vPilot receives a private message or connects to ATC, the plugin sends:



```csharp

SendMessage("NEW\_PRIVATE\_MESSAGE");

```



Homebridge receives the message and activates a motion sensor in HomeKit.



---



\## 🧪 Debugging



All debug output is sent via `SendDebug()` and visible in the vPilot plugin console. Type .debug in the vpilot message window to display



---



\## 📜 License



MIT – see \[LICENSE](LICENSE)



---



\## 🤝 Credits



Developed by Montislignum
Please support me if you like the plugin

Built to integrate vPilot with HomeKit via TCP and Homebridge

```



