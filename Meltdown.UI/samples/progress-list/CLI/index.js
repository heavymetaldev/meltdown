import { cli } from "./build/cli.js";
import { commandEmitter } from "./build/lib/utils/commands.js";

//import dotnet from "node-api-dotnet/net8.0";

//dotnet.addListener("resolving", (name, version, resolve) => {
//  const filePath = path.join(
//    __dirname,
//    "bin",
//    "Debug",
//    "net8.0",
//    name + ".dll"
//  );
//  if (fs.existsSync(filePath)) resolve(filePath);
//});

//const HmDev = dotnet.require("../bin/Debug/net8.0/HMDev.NodeUI");

//const result = HmDev.DirectCommandDispatcher.invoke("tests", "me", ["trala"]);

//declare function invokeCommand(command: string, path: string);

export {
  cli,
  commandEmitter
}