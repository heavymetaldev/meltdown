import cli from "./cli.js";

cli.commandEmitter.on("invoke", (command, path, args) => {
  cli.progressEmitter.log("commands", `Command: [${path}]:${command} [${args}](${args.length})`);
});

cli.cli({
  init: () => {
    console.log("Hello World!");
    cli.progressEmitter.log(
      "init",
      "App initalizing..."
    );
    cli.progressEmitter.update("init", {
      progress: 5,
    })
    cli.progressEmitter.setCommands("init", JSON.stringify([
      {
        name: "start",
        description: "Start the app",
        key: "s",
      },
      {
        name: "stop",
        description: "Stop the app",
        key: "x",
      }
    ]));
    
    cli.commandEmitter.on("invoke", (command, path, args) => {
      switch (command) {
        case "start":
          cli.progressEmitter.log(
            "init",
            "Starting the app..."
          );
          cli.progressEmitter.update("init", {
            status: "Starting",
            state: "running",
            progress: 50,
          });
          break;
        case "stop":
          cli.progressEmitter.log(
            "init",
            "Stopping the app..."
          );
          cli.progressEmitter.update("init", {
            status: "Stopped",
            state: "stopped",
            progress: 100,
          });
          break;
        default:
          cli.progressEmitter.log(
            "init",
            `Unknown command: ${command}`
          );
          break;
      }
    });

    setTimeout(() => {
      cli.progressEmitter.log(
      "init",
      "Init DONE."
    );
      cli.progressEmitter.update("init", {
        state: "done",
        status: "Running",
        progress: 100,
      });
    }
    , 1000);
  },
  variant: "treeview",
});

// cli({
//     init: () => {
//         console.log("CLI initialized");
//         let i = 0;
//         setInterval(() => {
//             console.log(`heartbeat ${i++}`);
//             progressEmitter.update(`beat|${i}`, {
//                 state: "running",
//                 status: "beating",
//                 details: `beat.${i}`,
//             });
//             progressEmitter.update(`beat`, {
//                 state: "running",
//                 status: "beating",
//                 details: `beat.${i}`,
//                 progress: i,
//             });
//         }, 100);
//     },
//     variant: "master-detail",
// });
