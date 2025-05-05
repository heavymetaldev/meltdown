import { cli, commandEmitter, progressEmitter } from "./cli.js";


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