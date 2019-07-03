import { Action as ReduxAction, createStore } from 'redux';
import { Config } from '@/types/Config';

export enum ActionTypes {
    APP_READY = 'APP_READY',
    NAVIGATE = 'NAVIGATE',
    UPDATE_SERVICE = 'UPDATE_SERVICE',
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
    config: Config;
};

type AppReadyAction = ReduxAction<ActionTypes.APP_READY>;

interface NavigateAction extends ReduxAction<ActionTypes.NAVIGATE> {
    navigateTo: string;
}

interface UpdateConfigAction extends ReduxAction<ActionTypes.UPDATE_SERVICE> {
    status: ServiceStatus;
    config: Config;
}

export type Action =
    | AppReadyAction
    | NavigateAction
    | UpdateConfigAction
    ;

const initState: GlobalState = {
    ready: false,
    location: 'overview',
    status: ServiceStatus.Starting,
    config: {
        'port': 0,
        'socket-port': 0,
        'redir-port': 0,
        'allow-lan': false,
        'mode': 'Rule',
        'log-level': 'info',
        'external-controller': 'localhost:9090',
    },
};

function reducer(state: GlobalState = initState, action: Action): GlobalState {
    switch (action.type) {
        case ActionTypes.APP_READY:
            return { ...state, ready: true };

        case ActionTypes.NAVIGATE:
            return { ...state, location: action.navigateTo };

        case ActionTypes.UPDATE_SERVICE:
            return { ...state, status: action.status, config: action.config };

        default:
            return state;
    }
}

export default createStore(reducer);
