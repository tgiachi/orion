# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="0.11.1"></a>
## [0.11.1](https://www.github.com/tgiachi/orion/releases/tag/v0.11.1) (2025-04-28)

<a name="0.11.0"></a>
## [0.11.0](https://www.github.com/tgiachi/orion/releases/tag/v0.11.0) (2025-04-28)

### Features

* **.gitignore:** add src/Orion.Server/Assets/Web to .gitignore to exclude it from version control ([6d2733c](https://www.github.com/tgiachi/orion/commit/6d2733c6dfc455a96b320f9207169b153d121c80))
* **apiStore.ts:** add ApiStore class to handle API requests and error handling ([88089de](https://www.github.com/tgiachi/orion/commit/88089de923f9cef2c1f03b595bf192f446a49220))
* **App.tsx:** add DashboardPage component and route to /dashboard path ([b7e7b15](https://www.github.com/tgiachi/orion/commit/b7e7b1505a12d92b448aeb6a724d2c84818fcfe0))
* **App.tsx:** refactor App component to use react-router-dom for routing ([3944bc9](https://www.github.com/tgiachi/orion/commit/3944bc91bde245859671cce31e5e1ae979cf7327))
* **ChannelData.cs:** add Members property to store channel members list for easier access ([87fa7f1](https://www.github.com/tgiachi/orion/commit/87fa7f1ebede4481b4274cc6f689c03fe507dbe0))
* **ChannelsHandler.cs:** add support for handling KickCommand to manage kicking users from channels ([e9bfbdd](https://www.github.com/tgiachi/orion/commit/e9bfbdd7ff386b82ff887849225e910d1259f629))
* **Dockerfile:** add npm install and build steps for orion-web-ui to support Vite ([d07fd14](https://www.github.com/tgiachi/orion/commit/d07fd1498e0e5abbadc2722ba0c7b34b5505b38b))
* **IrcUserSession.cs:** add IsSecureConnection property to track secure connections ([11f6621](https://www.github.com/tgiachi/orion/commit/11f66217b47455256f9081f594563ab3c73ae3a4))
* **orion-web-ui:** add new dependencies @emotion/styled, @mui/material, and @mui/x-date-pickers ([ad91065](https://www.github.com/tgiachi/orion/commit/ad91065e6513ba271495f37aec5af21653ec454a))
* **orion-web-ui:** add postcss configuration for tailwindcss and autoprefixer ([9808ba1](https://www.github.com/tgiachi/orion/commit/9808ba193aa03a773b82e258e737d3daaaaba4c1))
* **orion-web-ui:** update dependencies in package.json to use caret (^) for version ranges ([e58754f](https://www.github.com/tgiachi/orion/commit/e58754fba68a8cb3c4b6cc4e329f5ff83c03f219))
* **orion-web-ui:** update package.json dependencies and devDependencies ([a0574e7](https://www.github.com/tgiachi/orion/commit/a0574e7341a8300de100be152b5093426ba5e0ec))
* **VersionStatus.tsx:** update VersionStatus component to display the version information ([4cba241](https://www.github.com/tgiachi/orion/commit/4cba241c65abe25487e21f1c0b1ec8b350babef5))

### Bug Fixes

* **csproj:** update Serilog.Sinks.File package version to 7.0.0 for Orion.Core.Server project ([5fd64ad](https://www.github.com/tgiachi/orion/commit/5fd64adf945a7d0bdbcd93486c51f26b0881cd49))
* **index.html:** update favicon path to be relative for better file structure ([d8f11b7](https://www.github.com/tgiachi/orion/commit/d8f11b7aa5d35169b13dc3f0af6b19ae90096043))

<a name="0.10.0"></a>
## [0.10.0](https://www.github.com/tgiachi/orion/releases/tag/v0.10.0) (2025-04-23)

### Features

* **ChannelEntry.cs:** add ChannelEntry record for representing channel data ([1c05bfa](https://www.github.com/tgiachi/orion/commit/1c05bfa0a95cffcb20a9b7d34b5dd19dc596f47e))
* **WhoHandler.cs:** add WhoHandler class to handle WhoCommand and WhoIsCommand ([da83bd8](https://www.github.com/tgiachi/orion/commit/da83bd843149ae0fb35256d29c23633d3697076a))

<a name="0.9.0"></a>
## [0.9.0](https://www.github.com/tgiachi/orion/releases/tag/v0.9.0) (2025-04-23)

### Features

* **ChannelJoinResult.cs:** change AddJoinedUserCommand method signature to accept multiple commands ([74b1bc5](https://www.github.com/tgiachi/orion/commit/74b1bc5d6f069ed560d295db4fbf7870cb9c1b74))
* **IChannelManagerService.cs:** add method GetConnectedUsersAsync to retrieve connected users ([668e728](https://www.github.com/tgiachi/orion/commit/668e728f705d1e6a31a90a4ae693f5c9be6d960e))
* **OperEntryConfig.cs:** add Id property with YamlIgnore and JsonIgnore attributes ([8535df2](https://www.github.com/tgiachi/orion/commit/8535df290320bdb6d1219d81ec410e9f02d4d07c))
* **OperHandler.cs:** add IChannelManagerService dependency to OperHandler constructor for managing channel users and improve modularity ([2e9c62d](https://www.github.com/tgiachi/orion/commit/2e9c62d73ea4e45fc19e22a228ee4054b422f67b))
* **Program.cs:** add JWT authentication and authorization configuration for improved security ([a3410c1](https://www.github.com/tgiachi/orion/commit/a3410c10e80840fb29c974f93d9517c642aa82de))

<a name="0.8.0"></a>
## [0.8.0](https://www.github.com/tgiachi/orion/releases/tag/v0.8.0) (2025-04-22)

### Features

* **IrcUserSession.cs:** add IsLocal and RemoteServerId properties to improve S2S communication handling ([ce54a87](https://www.github.com/tgiachi/orion/commit/ce54a87fc456d2a4e6d75406559da0112d80a05a))
* **Orion.Core.Server:** add support for away functionality with AwayCommand ([ae4c704](https://www.github.com/tgiachi/orion/commit/ae4c704ab64a5049f593d83e3dc014510769e1ca))

<a name="0.7.0"></a>
## [0.7.0](https://www.github.com/tgiachi/Orion/releases/tag/v0.7.0) (2025-04-22)

### Features

* **ChannelDeletedEvent.cs:** add ChannelDeletedEvent record to represent a channel deletion event ([013d433](https://www.github.com/tgiachi/Orion/commit/013d4339f7e194c294aa78ec38778b51244f6fd9))
* **ChannelJoinResult.cs:** add ChannelJoinResult class to represent the result of joining a channel ([d614de1](https://www.github.com/tgiachi/Orion/commit/d614de1968cc77a1c7e390b8ad1062bc30ee581a))
* **ChannelJoinResult.cs:** add JoinedUserCommands and MembersCommands properties ([889499e](https://www.github.com/tgiachi/Orion/commit/889499eb5d29cff2bc92eb5161fef5774fd2ee48))
* **ErrNoRecipients.cs:** add new class ErrNoRecipients to handle ERR_NORECIPIENT error in IRC commands ([bb64b0a](https://www.github.com/tgiachi/Orion/commit/bb64b0a0ee6b1c7223932ce6c6d2f9ca1a0612ba))
* **Events:** add UserJoinChannelEvent, UserPrivateMessageEvent, and UserQuitEvent records to handle IRC user events ([63f1815](https://www.github.com/tgiachi/Orion/commit/63f18158f28c8f0d1f133e81efea0ea9dfd076d2))
* **IrcSessionService.cs:** add FindByNickName method to retrieve an IRC user session by nickname ([8ef41b3](https://www.github.com/tgiachi/Orion/commit/8ef41b31aae533009dee82e80168afb922c84141))
* **ITextTemplateService.cs:** add method GetVariablesAndContent to retrieve all variables and their content ([545fd9c](https://www.github.com/tgiachi/Orion/commit/545fd9c492e84bf0b6fef101d5ad52d5f6d74b6f))
* **NoticeCommand.cs:** add new property TargetType to determine the type of target for notice messages ([3ab2b62](https://www.github.com/tgiachi/Orion/commit/3ab2b628641d2688896ed540667ee49713e3df41))
* **README.md:** rename Orion.Core to Orion.Foundations for better clarity and consistency ([8545f57](https://www.github.com/tgiachi/Orion/commit/8545f577e00a2b142ff55f2cad55ca1ed9ec7865))
* **UserPrivateMessageEvent.cs:** add UserPrivateMessageEvent record to handle private messages in IRC ([fa01e0d](https://www.github.com/tgiachi/Orion/commit/fa01e0d103f1c74a2a6100bf7c1d0eaac8a0efc4))
* **UserPrivMessageHandler.cs:** add check for maximum targets limit to prevent sending messages to too many users at once ([91c4672](https://www.github.com/tgiachi/Orion/commit/91c4672ea40360826b79f03f6180650795c7c805))
* **UserPrivMessageHandler.cs:** add UserPrivMessageHandler class to handle private messages between users ([61cd19d](https://www.github.com/tgiachi/Orion/commit/61cd19de784e29b1a4ceb14968849f5d5d15c12b))

### Bug Fixes

* **ChannelsHandler.cs:** fix a bug where a member was not removed from the channel data when leaving a channel ([d7b9bf9](https://www.github.com/tgiachi/Orion/commit/d7b9bf9841ac4862b1be4bf2887fa9e4bfed0a87))

<a name="0.6.1"></a>
## [0.6.1](https://www.github.com/tgiachi/Orion/releases/tag/v0.6.1) (2025-04-20)

<a name="0.6.0"></a>
## [0.6.0](https://www.github.com/tgiachi/Orion/releases/tag/v0.6.0) (2025-04-20)

### Features

* **IrcServerConfig.cs:** add IrcSupportConfig class to manage IRC server limits ([b331259](https://www.github.com/tgiachi/Orion/commit/b331259c3a7eada7d6d5dc4e33a4bafdf01d51a6))
* **IrcServerConfig.cs:** add Motd property to store Message of the Day for the server ([206f9eb](https://www.github.com/tgiachi/Orion/commit/206f9eb81319e23c7a197fca0848b02e8739805e))
* **IrcServerContextData.cs:** add ServerStartTime property to store the start time of the server ([ed974f4](https://www.github.com/tgiachi/Orion/commit/ed974f4f898af876adc63a1f1692bc422fd87a35))
* **IrcUserSession.cs:** add IsAway and AwayMessage properties for managing away status ([198a196](https://www.github.com/tgiachi/Orion/commit/198a19638faa63bea768b4500fdf925a37f3e0a5))

### Bug Fixes

* **PingConfig.cs:** update default values for Interval and Timeout properties to improve ([e8caefc](https://www.github.com/tgiachi/Orion/commit/e8caefc94f5f7ce9c1f1f64fcec8d9974ec22013))
* **PingPongHandler.cs:** remove unnecessary ServerHostName variable usage in PingCommand creation ([a239bbf](https://www.github.com/tgiachi/Orion/commit/a239bbfe403a992390fe1ba9afca58e6d3d66df8))

<a name="0.5.1"></a>
## [0.5.1](https://www.github.com/tgiachi/Orion/releases/tag/v0.5.1) (2025-04-20)

<a name="0.5.0"></a>
## [0.5.0](https://www.github.com/tgiachi/Orion/releases/tag/v0.5.0) (2025-04-20)

### Features

* **IrcServerContextData.cs:** add NetworkName property to IrcServerContextData class ([b529d77](https://www.github.com/tgiachi/Orion/commit/b529d77b8d603553661b43c072c9c8ea7e6abbd9))
* **VersionInfoData.cs:** add VersionInfoData record to store version information for the app ([ca5cfb1](https://www.github.com/tgiachi/Orion/commit/ca5cfb1d99f8f7c431281a962fd2e06f538b5127))

<a name="0.4.0"></a>
## [0.4.0](https://www.github.com/tgiachi/Orion/releases/tag/v0.4.0) (2025-04-19)

### Features

* **EventBus:** implement EventBusConfig to control the maximum number of concurrent tasks used for event dispatching ([38fff2e](https://www.github.com/tgiachi/Orion/commit/38fff2eba36136660f57a0a72122bc269ce94166))

<a name="0.3.0"></a>
## [0.3.0](https://www.github.com/tgiachi/Orion/releases/tag/v0.3.0) (2025-04-18)

### Features

* **IrcServerConfig.cs:** add ServerPassword property to IrcServerConfig class ([ff790d1](https://www.github.com/tgiachi/Orion/commit/ff790d17c50e499c221d386e26d82ce68e4e855c))
* **OperEntryConfig.cs:** add methods to set and validate password, ensure password starts with 'hash://' ([684e7de](https://www.github.com/tgiachi/Orion/commit/684e7de957624d11949aa4b010d70da12a62ff2c))

### Bug Fixes

* **ConnectionHandler.cs:** add logging for when a nickname is already in use to provide visibility ([501b273](https://www.github.com/tgiachi/Orion/commit/501b273ade67c6046e2942dde5868c746aa01462))

<a name="0.2.0"></a>
## [0.2.0](https://www.github.com/tgiachi/Orion/releases/tag/v0.2.0) (2025-04-18)

### Features

* **IrcUserSession.cs:** add properties for user details and authentication status ([240719f](https://www.github.com/tgiachi/Orion/commit/240719feb366f155aacd12d90d5ea65994c37281))
* **OrionServerConfig.cs:** add IrcServerConfig and PingConfig classes for IRC server configuration ([20ab498](https://www.github.com/tgiachi/Orion/commit/20ab498e683769d2abecf98fed4bbff792c7df0a))

### Bug Fixes

* **IrcCommandParser.cs:** change newline character from Environment.NewLine to "\r\n" for consistency ([029ad40](https://www.github.com/tgiachi/Orion/commit/029ad406b697678e19c3a37157eeedc9476cca3d))

<a name="0.1.10"></a>
## [0.1.10](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.10) (2025-04-18)

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
* **workflows:** add release workflow to automate versioning and creating releases based on tags ([fbcc127](https://www.github.com/tgiachi/Orion/commit/fbcc1276fa088f9b899aa44f35092c7d6b2dd03b))

### Bug Fixes

* **OperEntryConfig.cs:** update PasswordHash property to include 'hash://' prefix ([4196d2e](https://www.github.com/tgiachi/Orion/commit/4196d2ec0c9b159323696697d68af2de3c47a107))
* **README.md:** update Docker image link to point to the correct repository name 'orionirc-server' for consistency and accuracy ([c1e84d0](https://www.github.com/tgiachi/Orion/commit/c1e84d0320538198e9f043198d1331beb6537855))

<a name="0.1.10"></a>
## [0.1.10](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.10) (2025-04-18)

<a name="0.1.9"></a>
## [0.1.9](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.9) (2025-04-18)

<a name="0.1.8"></a>
## [0.1.8](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.8) (2025-04-18)

<a name="0.1.7"></a>
## [0.1.7](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.7) (2025-04-18)

<a name="0.1.6"></a>
## [0.1.6](https://www.github.com/tgiachi/Orion/releases/tag/v0.1.6) (2025-04-18)

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

