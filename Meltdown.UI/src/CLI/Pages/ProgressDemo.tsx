import React, { useEffect, useState } from 'react';
import { Box, Text } from '../dependencies/tuir.js';
import { AnimatedProgressBar } from '../dependencies/ink-components.js';

export function ProgressDemo() {
    const [progress, setProgress] = useState(0);
    const [direction, setDirection] = useState(1);

    useEffect(() => {
        const interval = setInterval(() => {
            setProgress(prev => {
                const next = prev + (direction * 0.5); // Slower progress change
                if (next >= 100 || next <= 0) {
                    setDirection(d => -d);
                    return prev + (-direction * 0.5);
                }
                return next;
            });
        }, 50);

        return () => clearInterval(interval);
    }, [direction]);

    return (
        <Box flexDirection="column" padding={1}>
            <Box marginBottom={1}>
                <Text>Wave Animation (ripple effect):</Text>
            </Box>
            <Box marginBottom={2}>
                <AnimatedProgressBar
                    value={progress}
                    width={40}
                    style={{
                        animationStyle: 'wave',
                        animationSpeed: 50,
                        colors: { filled: 'cyan', empty: 'gray' }
                    }}
                />
            </Box>

            <Box marginBottom={1}>
                <Text>Pulse Animation (blinking):</Text>
            </Box>
            <Box marginBottom={2}>
                <AnimatedProgressBar
                    value={progress}
                    width={40}
                    style={{
                        animationStyle: 'pulse',
                        animationSpeed: 200,
                        colors: { filled: 'magenta', empty: 'gray' }
                    }}
                />
            </Box>

            <Box marginBottom={1}>
                <Text>Slide Animation (moving dot):</Text>
            </Box>
            <Box marginBottom={2}>
                <AnimatedProgressBar
                    value={progress}
                    width={40}
                    style={{
                        animationStyle: 'slide',
                        animationSpeed: 100,
                        colors: { filled: 'yellow', empty: 'gray' }
                    }}
                />
            </Box>

            <Box marginBottom={1}>
                <Text>Bounce Animation (bouncing dot):</Text>
            </Box>
            <Box>
                <AnimatedProgressBar
                    value={progress}
                    width={40}
                    style={{
                        animationStyle: 'bounce',
                        animationSpeed: 100,
                        colors: { filled: 'green', empty: 'gray' }
                    }}
                />
            </Box>
        </Box>
    );
} 