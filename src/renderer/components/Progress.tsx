import React from 'react';

import './Progress.scss';

type ProgressProps = {
    ratio: number;
};

const Progress: React.FunctionComponent<ProgressProps> = (props) => (
    <div className="progress">
        <div className="progress-bar">
            <div className="progress-indicator" style={{ width: `${props.ratio * 100}%` }} />
        </div>
    </div>
);

export default Progress;
