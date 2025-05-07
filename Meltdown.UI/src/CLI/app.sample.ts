import cli from "./cli.js";

cli.cli({
  init: () => {
    console.log("Hello World!");
    cli.progressEmitter.log(
      "init",
      "Initializing CLI..."
    );
    cli.progressEmitter.update("init", {
      progress: 99,
    });
  },
  variant: "master-detail",
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
