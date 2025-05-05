import { cli, progressEmitter, commandEmitter } from "./cli.js";

cli({
  init: () => {
    console.log("Hello World!");
   progressEmitter.log("init", `progress: ${progressEmitter.id} command:${commandEmitter.id}`);
   progressEmitter.update("init", {
    progress: 99,
   });
  },
  variant: "master-detail",
});
