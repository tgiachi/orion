# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="0.1.5"></a>
## [0.1.5](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.5) (2025-04-18)

<a name="0.1.4"></a>
## [0.1.4](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.4) (2025-04-17)

### Bug Fixes

* **README.md:** update Docker image link to point to the correct repository name 'orionirc-server' for consistency and accuracy ([c1e84d0](https://www.github.com/tgiachi/Orion/commit/c1e84d0320538198e9f043198d1331beb6537855))

<a name="0.1.3"></a>
## [0.1.3](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.3) (2025-04-17)

<a name="0.1.2"></a>
## [0.1.2](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.2) (2025-04-17)

<a name="0.1.1"></a>
## [0.1.1](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.1) (2025-04-17)

### Features

* actions, libs and docker ([362e5b9](https://www.github.com/tgiachi/Orion/commit/362e5b95332d273309dfb781b4ce0c529fbc3ac3))
* add Directory.Build.props file to include Nerdbank.GitVersioning package reference ([80879a9](https://www.github.com/tgiachi/Orion/commit/80879a9b09ee68df240da049a1494b5e2b04e6bc))
* moved from abyssirc to orion ([9f9c034](https://www.github.com/tgiachi/Orion/commit/9f9c034af2349326595bee726ead90f029ef6b14))
* **AddIrcCommandToParserExtension.cs:** add extension method to register IRC commands to parser ([c67de1b](https://www.github.com/tgiachi/Orion/commit/c67de1b3cb1fb510afdedc985449cac82e5a76ea))
* **AppContextData.cs, OrionServerOptions.cs, ConfigLoaderExtension.cs, OptionBuilderExtension.cs, IOrionServerCmdOptions.cs, Orion.Core.Server.csproj:** add new classes and extensions for handling application context data, server options, configuration loading, and option building to improve modularity and maintainability of the server codebase. ([055466a](https://www.github.com/tgiachi/Orion/commit/055466ab1539cd007d062c474d979b2c17ae2a13))
* **BaseIrcCommandListener.cs:** add support for IHyperPostmanService and IIrcSessionService dependencies in the constructor to enhance functionality ([1a2330b](https://www.github.com/tgiachi/Orion/commit/1a2330bb8d8ff5185905fcfcfc925b6e699a1d4b))
* **config:** add BaseConfigSection as a base class for config sections to avoid code duplication ([e616b77](https://www.github.com/tgiachi/Orion/commit/e616b77ca094a0175522d01f23b48e112dcb299a))
* **DefaultServicesModule.cs:** add DefaultServicesModule to register default services in the Orion container ([8ab163c](https://www.github.com/tgiachi/Orion/commit/8ab163ccceb7b35e08418c5b073aee5cd5cd55e8))
* **Dockerfile:** add Dockerfile to set up multi-stage build for the Orion Server application ([ad73da5](https://www.github.com/tgiachi/Orion/commit/ad73da538962e3e760a3d38ae14dbe0fc6731466))
* **IIrcCommandListener.cs:** add IIrcCommandListener interface to define a contract for handling IRC commands ([7dab021](https://www.github.com/tgiachi/Orion/commit/7dab021f52952cb9aa87e8baf583172d67580664))
* **IIrcCommandListener.cs:** create IIrcCommandListener interface to handle received commands ([4485831](https://www.github.com/tgiachi/Orion/commit/4485831b95c75f22f6fb4f748b08cdef00fc8744))
* **IrcListenerDefinition.cs:** add new file IrcListenerDefinition to define IRC listener type ([2427c1f](https://www.github.com/tgiachi/Orion/commit/2427c1f21b04e40fa7d62b982e300ada24b99946))
* **IrcUserSession.cs:** add a new class IrcUserSession to manage IRC user sessions ([1fe8842](https://www.github.com/tgiachi/Orion/commit/1fe884287c37be43359cd1f0f8ba97b46186e121))
* **Network:** add UseWebSocket property to NetworkBindConfig for WebSocket support ([c1ccb7a](https://www.github.com/tgiachi/Orion/commit/c1ccb7a142e4cdb41ddc782ad3f9b868e0d911f7))
* **NetworkService:** add NetworkService to handle network-related functionality ([370ff95](https://www.github.com/tgiachi/Orion/commit/370ff95de9e4bbfc966dd0d549cf8429c1c97ae2))
* **Orion.Core.Server:** add new server events and text template services ([dcf88c5](https://www.github.com/tgiachi/Orion/commit/dcf88c5430e8386ffc91ba65c7157e98fc5fb598))
* **Orion.Core.Server:** add SessionConnectedEvent and SessionDisconnectedEvent classes for IRC sessions to handle session connections and disconnections ([84135bd](https://www.github.com/tgiachi/Orion/commit/84135bdeb8af3ef73b03261d211a2a29360ab7f8))
* **Orion.Core.Server.csproj:** add HyperCube.Postman package reference to enable integration with Postman for API testing ([fe6afc6](https://www.github.com/tgiachi/Orion/commit/fe6afc652c0ab25822c773bf944f3bdb4baf3679))
* **Orion.Network.Tcp.csproj:** add version 0.1.0 to the project for tracking and identification purposes ([78ee575](https://www.github.com/tgiachi/Orion/commit/78ee5750fb9445122f37cbdac420e13e48c5645b))
* **Orion.Server.csproj:** add ProjectReference to Orion.Irc.Core.csproj for IRC functionality integration ([15ea7a0](https://www.github.com/tgiachi/Orion/commit/15ea7a0d486627a0e875b115dab779181b5f9715))
* **Orion.sln:** add Orion.Core.Server.Web project to the solution ([3a93b56](https://www.github.com/tgiachi/Orion/commit/3a93b561e93a82f1b1df93bcc87fa017a3069629))
* **Orion.sln:** add Orion.Irc.Core project to the solution ([7097840](https://www.github.com/tgiachi/Orion/commit/7097840d3df9c22349b88f7f7930507a7e17ae6a))
* **Orion.sln:** add Orion.Network.Tcp project to the solution ([efcd224](https://www.github.com/tgiachi/Orion/commit/efcd22418b70057eb24839a4aa4b11e8858a0734))
* **OrionHostedService.cs:** add logging message when server starts and is ready to improve monitoring and debugging ([cace467](https://www.github.com/tgiachi/Orion/commit/cace467a0c3aecf10a2d385d35827d5d9d87a31b))
* **OrionServerConfig.cs:** add a new property to OrionServerConfig class ([8ad7e35](https://www.github.com/tgiachi/Orion/commit/8ad7e35ba16d396e51975228e9a041207c7e99ba))
* **OrionServerConfig.cs:** add DebugConfig section to OrionServerConfig for debugging purposes ([11bedc7](https://www.github.com/tgiachi/Orion/commit/11bedc75bab746e7f6d32bdd91809c1d79c48cb2))
* **OrionServerConfig.cs:** add NetworkConfig property to OrionServerConfig for managing network configurations ([b374977](https://www.github.com/tgiachi/Orion/commit/b3749775e31529a0e4187a184e8252e30a41ba86))
* **OrionServerConfig.cs:** add OrionServerConfig class to manage server configurations ([cf53d97](https://www.github.com/tgiachi/Orion/commit/cf53d97c4453138d2ac46f12b205662237ac985c))
* **OrionServerConfig.cs:** add PidFile property to store the process ID file path ([f8ffb5b](https://www.github.com/tgiachi/Orion/commit/f8ffb5b7e0e2650253e221e8c9198a1eb7678668))
* **OrionServerConfig.cs:** refactor OrionServerConfig to have individual properties for each section ([5cc9f80](https://www.github.com/tgiachi/Orion/commit/5cc9f8099a1c7d25e01d21a5540bf6101eb2f8ea))
* **OrionServerConfig.cs:** remove JwtAuthConfig property to simplify configuration ([68d25f2](https://www.github.com/tgiachi/Orion/commit/68d25f27a4267d36639afd8a71eb9cade50d16ab))
* **README.md:** add initial version, license, and .NET version badges for project ([51375bf](https://www.github.com/tgiachi/Orion/commit/51375bfc678c4a1896ac512f051243bb5734e33f))
* **server:** add LogLevelExtensions to convert custom log levels to Serilog log levels ([39438ca](https://www.github.com/tgiachi/Orion/commit/39438ca241fcef28d7285b9fd8a0d5ede32d0cd2))
* **server:** add ScriptModuleData class to handle script module data ([009e278](https://www.github.com/tgiachi/Orion/commit/009e2782c92b22d7c5560766c85a899d96060f7b))
* **server.ts:** add support for process.env.PORT environment variable to be able to run app on a configurable port ([dcbfa1a](https://www.github.com/tgiachi/Orion/commit/dcbfa1a11e8a0982f00820ccb58ea2ef2ab57ab6))

### Bug Fixes

* **OperEntryConfig.cs:** update PasswordHash property to include 'hash://' prefix ([4196d2e](https://www.github.com/tgiachi/Orion/commit/4196d2ec0c9b159323696697d68af2de3c47a107))

