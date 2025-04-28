import React, { PropsWithChildren } from "react";
import { Viewport, Box, Text } from "tuir";
import { progressEmitter } from "./utils/commands.js";

export default function App(props: Parameters<typeof Box>[0]) {
  const [log, setLog] = React.useState<string[]>([]);

  React.useEffect(() => {
    const logListener = (path: string, message: string) => {
      setLog((prevLog) => [...prevLog, `[${path}] ${message}`]);
    };

    progressEmitter.on("log", logListener);

    return () => {
      progressEmitter.off("log", logListener);
    };
  }, []);

  return <Box {...props}><Text>{log.join("\n")}</Text></Box>;
}
