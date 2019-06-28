import React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './Content.scss';
import { Action, ActionTypes, GlobalState } from '@/state';
import Overview from './Overview';

function renderCurrentContent(current: string) {
    switch (current) {
        case 'overview': return <Overview />;
        case 'proxy': return <Overview />;
        case 'profile': return <Overview />;
        case 'log': return <Overview />;
        case 'settings': return <Overview />;
        case 'help': return <Overview />;
        case 'about': return <Overview />;
        default: return null;
    }
}

type ContentProps = {
    current: string;
};

const Content: React.FunctionComponent<ContentProps> = (props) => (
    <div className="content">
        {renderCurrentContent(props.current)}
    </div>
);

const mapStateToProps = (state: GlobalState) => ({
    current: state.currentNavItem,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Content);
