import React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import './Content.scss';
import { Action, ActionTypes, GlobalState } from '../store';
import Overview from './Overview';

type ContentProps = {
    location: string;
};

const Content: React.FunctionComponent<ContentProps> = (props) => (
    <div className="content">
        <Overview isVisible={props.location === 'overview'} />
    </div>
);

const mapStateToProps = (state: GlobalState) => ({
    location: state.location,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>) => ({});

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Content);
