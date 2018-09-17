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

Easy enough? Try it and freak it out!
