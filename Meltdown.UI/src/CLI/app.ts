import cli from "./cli.js";

cli.cli({
  init: () => {
    console.log("Hello World!");
    cli.progressEmitter.log("init", "Initializing CLI...");
    cli.progressEmitter.update("init", {
      progress: 99,
    });
  },
  variant: "master-detail",
});
