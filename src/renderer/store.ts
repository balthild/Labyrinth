import { Action as ReduxAction, createStore } from 'redux';
import { ClashConfig } from '@/types/ClashConfig';
import { ClashController } from '@/types/ClashController';

export enum ActionTypes {
    INITIALIZE = 'APP_READY',
    UPDATE_CONFIG = 'UPDATE_CONFIG',
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
    controller: ClashController;
};

interface InitializeAction extends ReduxAction<ActionTypes.INITIALIZE> {
    controller: ClashController;
}

interface UpdateConfigAction extends ReduxAction<ActionTypes.UPDATE_CONFIG> {
    status: ServiceStatus;
    config: ClashConfig;
}

interface NavigateAction extends ReduxAction<ActionTypes.NAVIGATE> {
    navigateTo: string;
}

export type Action =
    | NavigateAction
    | InitializeAction
    | UpdateConfigAction
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
    controller: {
        'external-controller': '',
        'secret': '',
    },
    location: 'overview',
};

function reducer(state: GlobalState = initState, action: Action): GlobalState {
    switch (action.type) {
        case ActionTypes.INITIALIZE:
            return { ...state, ready: true, controller: action.controller };

        case ActionTypes.UPDATE_CONFIG:
            return { ...state, status: action.status, config: action.config };

        case ActionTypes.NAVIGATE:
            return { ...state, location: action.navigateTo };

        default:
            return state;
    }
}

export default createStore(reducer);
