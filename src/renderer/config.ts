import fs from 'fs-extra';
import yaml from 'js-yaml';

import { configDirPath, getConfigFilePath } from '@/util';
import { Config } from '@/types/Config';
import { ClashController } from '@/types/ClashController';
import { ClashConfig } from '@/types/ClashConfig';

export async function getAppConfig(): Promise<Config> {
    let path = getConfigFilePath('labyrinth.json');

    const defaultConfig: Config = {
        configName: 'config.yaml',
    };

    if (!await fs.pathExists(path)) {
        await fs.writeFile(path, JSON.stringify(defaultConfig));
        return defaultConfig;
    }

    const data = await fs.readFile(path);
    const config: Config = {
        ...defaultConfig,
        ...JSON.parse(data.toString()),
    };

    const clashConfigNames = (await fs.readdir(configDirPath))
        .filter(name => name.endsWith('yml') || name.endsWith('.yaml'));

    if (!clashConfigNames.includes(config.configName)) {
        config.configName = 'config.yaml';
        await fs.writeFile(path, config);
    }

    return config;
}

export async function getClashConfig(name: string): Promise<ClashConfig> {
    const path = getConfigFilePath(name);
    const data = await fs.readFile(path);
    return yaml.safeLoad(data.toString());
}

export async function getClashController(): Promise<ClashController> {
    const path = getConfigFilePath('config.yaml');

    let exists = await fs.pathExists(path);
    if (!exists) {
        const oldPath = getConfigFilePath('config.yml');
        if (await fs.pathExists(oldPath)) {
            exists = true;
            await fs.move(oldPath, path);
        }
    }

    if (exists) {
        try {
            const data = await fs.readFile(path);
            const config = yaml.safeLoad(data.toString());

            if (!config['external-controller']) {
                config['external-controller'] = 'localhost:9090';
                await fs.writeFile(path, yaml.safeDump(config));
            }

            return {
                'external-controller': config['external-controller'],
                'secret': config.secret,
            };
        } catch {
            await fs.unlink(path);
        }
    }

    await fs.writeFile(path, require('@/data/config.yaml'));

    return {
        'external-controller': '127.0.0.1:9090',
        'secret': '',
    };
}
