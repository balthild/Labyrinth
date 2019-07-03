import React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import { Action, ActionTypes, GlobalState } from '../store';

type Props = {
    current: string;
    name: string;
    icon: string;
    text: string;
    navigate(to: string): void;
};

class NavigationItem extends React.Component<Props> {
    isCurrent() {
        const { current, name } = this.props;
        return current === name;
    }

    onClick = () => {
        if (!this.isCurrent()) {
            this.props.navigate(this.props.name);
        }
    };

    render() {
        const { icon, text } = this.props;

        return (
            <li className={this.isCurrent() ? 'current' : ''} onClick={this.onClick}>
                <i className={'ion ion-ios-' + icon} />
                <span>{text}</span>
            </li>
        );
    }
}

const mapStateToProps = (state: GlobalState) => ({
    current: state.location,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({
    navigate(to: string) {
        dispatch({
            type: ActionTypes.NAVIGATE,
            navigateTo: to,
        });
    },
});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(NavigationItem);
