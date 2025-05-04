import { cli, progressEmitter, commandEmitter } from "./cli.js";

cli({
  init: () => {
    progressEmitter.log("init", "Hello World!");
    progressEmitter.log("init", `progress: ${progressEmitter.id} command:${commandEmitter.id}`);
  },
});
