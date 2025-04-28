import React, { PropsWithChildren } from "react";
import { Viewport, Box, Text } from "tuir";
import { progressEmitter } from "./utils/commands.js";

export default function App(props: Parameters<typeof Viewport>[0]) {
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

  return (
    <Viewport {...props} flexDirection="column">
      <Box
        flexDirection="column"
        borderColor="green"
        borderStyle="round"
        padding={1}
      >
        <Text>{log.join("\n")}</Text>
      </Box>
    </Viewport>
  );
}
