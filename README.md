# NetworkSniffer
Transparent proxy built in a .Net Core 3.1 Console Application to monitor traffic levels and provide insight into traffic patterns.

## Setup
This project can be run from Visual Studio or published and run as an executable. That executable is located in Google Drive [here](https://drive.google.com/drive/folders/1A3ApmAgl4vDKW1umG6FoKDVHaxtUU9HA?usp=sharing "Network Sniffer Executable") and includes the configurable appsettings. It has been compiled to run on a win-x86 runtime self-contained.

The port the proxy is running on can be adjusted in the appsettings.json file, where you can also tweak 
the threshold metric that will trigger high traffic alarms, and the time period used to traffic average traffic.

Using the network sniffer will require further configuring the proxy settings on your machine. Windows instructions can
be found [here](https://docs.microsoft.com/en-us/microsoft-365/security/defender-endpoint/configure-proxy-internet?view=o365-worldwide "Configure Windows Proxy").

Mac instructions can be found [here](https://support.apple.com/guide/mac-help/enter-proxy-server-settings-on-mac-mchlp2591/mac "Configure Mac Proxy").

To test the traffic alert, lower the threshold and time settings.

## Logging
Logging has been configured with Seralog, and rolling log files will be written to the /logs directory when running the application. These logs include the ability to configure logging levels and log all proxied requests.

### Development
* Windows
Visual Studio 2019 as IDE for .NET Framework/.NET Core


### Example

![App Screenshot](https://github.com/jordvaug/NetworkSniffer/blob/main/Demo.JPG)
