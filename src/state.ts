import { Action as ReduxAction } from 'redux';

export enum ActionTypes {
    NAVIGATE = 'NAVIGATE',
}

export type GlobalState = {
    currentNavItem?: string;
};

export interface Action extends ReduxAction<ActionTypes> {
    navigateTo: string;
}

const initState: GlobalState = {
    currentNavItem: 'overview',
};

export function reducer(state: GlobalState = initState, action: Action): GlobalState {
    switch (action.type) {
        case ActionTypes.NAVIGATE:
            return { ...state, currentNavItem: action.navigateTo };

        default:
            return state;
    }
}
