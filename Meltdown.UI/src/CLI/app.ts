import { cli, progressEmitter, commandEmitter } from "./cli.js";

cli({
  init: () => {
    console.log("Hello World!");
   progressEmitter.log("init", `progress: ${progressEmitter.id} command:${commandEmitter.id}`);
  },
  variant: "master-detail",
});
