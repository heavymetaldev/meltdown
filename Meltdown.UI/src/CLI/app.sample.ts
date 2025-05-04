import { cli, progressEmitter } from "./cli.js";

cli({
    init: () => {
        console.log("CLI initialized");
        let i = 0;
        setInterval(() => {
            console.log(`heartbeat ${i++}`);
            progressEmitter.update("console", {
                state: "running",
                status: "beating",
                details: `beat.${i}`,
                progress: i / 10,
            });
        }, 100);
    },
    variant: "master-detail",
});