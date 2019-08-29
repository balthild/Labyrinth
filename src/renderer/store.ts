import { Action as ReduxAction, createStore } from 'redux';
import { ClashConfig } from '@/types/ClashConfig';
import { ClashController } from '@/types/ClashController';
import { Config } from '@/types/Config';

export enum ActionTypes {
    SERVICE_READY = 'SERVICE_READY',
    UPDATE_CONFIG = 'UPDATE_CONFIG',
    UPDATE_APP_CONFIG = 'UPDATE_APP_CONFIG',
    NAVIGATE = 'NAVIGATE',
}

export enum ServiceStatus {
    Starting,
    Running,
    Stopped,
}

export type GlobalState = {
    ready: boolean;
    location: string;
    status: ServiceStatus;
    config: ClashConfig;
    appConfig: Config;
    controller: ClashController;
};

interface ServiceReadyAction extends ReduxAction<ActionTypes.SERVICE_READY> {
    controller: ClashController;
}

interface NavigateAction extends ReduxAction<ActionTypes.NAVIGATE> {
    navigateTo: string;
}

interface UpdateConfigAction extends ReduxAction<ActionTypes.UPDATE_CONFIG> {
    status: ServiceStatus;
    config: ClashConfig;
}

interface UpdateAppConfigAction extends ReduxAction<ActionTypes.UPDATE_APP_CONFIG> {
    appConfig: Config;
}

export type Action =
    | ServiceReadyAction
    | NavigateAction
    | UpdateConfigAction
    | UpdateAppConfigAction
    ;

const initState: GlobalState = {
    ready: false,
    status: ServiceStatus.Starting,
    config: {
        'port': 0,
        'socks-port': 0,
        'redir-port': 0,
        'allow-lan': false,
        'mode': 'Rule',
        'log-level': 'info',
    },
    appConfig: {
        configFile: '',
    },
    controller: {
        'external-controller': '',
        'secret': '',
    },
    location: 'overview',
};

function reducer(state: GlobalState = initState, action: Action): GlobalState {
    switch (action.type) {
        case ActionTypes.SERVICE_READY:
            return { ...state, ready: true, controller: action.controller };

        case ActionTypes.UPDATE_CONFIG:
            return { ...state, status: action.status, config: action.config };

        case ActionTypes.UPDATE_APP_CONFIG:
            return { ...state, appConfig: action.appConfig };

        case ActionTypes.NAVIGATE:
            return { ...state, location: action.navigateTo };

        default:
            return state;
    }
}

export default createStore(reducer);
