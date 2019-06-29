import { Action as ReduxAction, createStore } from 'redux';

export enum ActionTypes {
    NAVIGATE = 'NAVIGATE',
}

export type GlobalState = {
    currentNavItem?: string;
    apiListen: string;
};

export interface Action extends ReduxAction<ActionTypes> {
    navigateTo: string;
}

const initState: GlobalState = {
    currentNavItem: 'overview',
    apiListen: '127.0.0.1:9090',
};

export function reducer(state: GlobalState = initState, action: Action): GlobalState {
    switch (action.type) {
        case ActionTypes.NAVIGATE:
            return { ...state, currentNavItem: action.navigateTo };

        default:
            return state;
    }
}

export default createStore(reducer);
