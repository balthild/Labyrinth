import path from 'path';
import os from 'os';
import http from 'http';

export const configDirName = 'clash';

export const configDirPath = path.join(os.homedir(), '.config', configDirName);

export function getConfigFilePath(filename: string) {
    return path.join(configDirPath, filename);
}

export function downloadFile(url: string, onProgress: (progress: number) => void): Promise<Buffer> {
    return new Promise((resolve, reject) => {
        http.get(url, (response) => {
            const totalSize = parseInt(response.headers['content-length']!!, 10);
            let downloadedSize = 0;

            const chunks: Buffer[] = [];
            response.on('data', (chunk: Buffer) => {
                downloadedSize += chunk.length;
                chunks.push(chunk);

                onProgress(downloadedSize / totalSize);
            });

            response.on('end', () => {
                resolve(Buffer.concat(chunks));
            });

            response.on('error', reject);
        });
    });
}
