# DotNet-Deployer
Deploy your .NET Project to Raspberry Pi with ease!
This will help you deploy your NET Core applications (Console or ASP.NET) to Raspbian in the blink of an eye. 

# Requirements

- A Raspberry Pi with Raspbian
- [.NET Core 2.1](https://www.microsoft.com/net/download)
- SSH enabled on the Pi
- Your Pi should accessible through your network

# Installation
- Open a command prompt
- Run `dotnet tool install --global SuperJMN.DotNet.Ssh`
- You're ready to go!

# How to deploy your application?

After installing the tool, 
- open a command prompt on the folder where your .csproj is located
- run `dotnet ssh deploy`. It will try to deploy with the default options
 * Default options: 
    - Your RPi's hostname is 'raspberrypi', 
    - the username is 'pi'  
    - and the password is 'raspberry'
- If you aren't using the defaults, you can specify each argument using the appropriate arguments with the command:
  * `dotnet ssh deploy --host=MyRaspberry --username=testuser --password=1234`

# Running your application
- You should install .NET Core into your Raspberry Pi :)
- Follow these instructions to install it inside Raspbian:

```
sudo apt-get -y update
sudo apt-get -y install libunwind8 gettext
wget https://dotnetcli.blob.core.windows.net/dotnet/Sdk/2.1.300/dotnet-sdk-2.1.300-linux-arm.tar.gz
wget https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/2.1.0/aspnetcore-runtime-2.1.0-linux-arm.tar.gz
sudo mkdir /opt/dotnet
sudo tar -xvf dotnet-sdk-2.1.300-linux-arm.tar.gz -C /opt/dotnet/
sudo tar -xvf aspnetcore-runtime-2.1.0-linux-arm.tar.gz -C /opt/dotnet/
sudo ln -s /opt/dotnet/dotnet /usr/local/bin
dotnet --info
```

Easy enough? Try it and freak it out!
