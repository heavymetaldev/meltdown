import React from "react";
import { Box, Cli, KeyMap, Text, useKeymap, Viewport } from "tuir";

export function Root() {
  const keyMap = {
    quit: { input: "q" },
  } satisfies KeyMap;
  const { useEvent } = useKeymap(keyMap);

  useEvent("quit", () => {
    process.exit(0);
  });

  return (
    <Viewport flexDirection="column">
      <Box
        flexDirection="column"
        borderColor="green"
        borderStyle="round"
        padding={1}
      >
        <Text>{"Hello world!"}</Text>
      </Box>
    </Viewport>
  );
}
