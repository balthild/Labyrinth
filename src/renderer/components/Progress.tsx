import React from 'react';

import './Progress.scss';

type ProgressProps = {
    ratio: number;
    className?: string;
    style?: React.CSSProperties;
};

const Progress: React.FC<ProgressProps> = (props) => (
    <div className={'progress ' + (props.className || '')} style={props.style}>
        <div className="progress-bar">
            <div className="progress-indicator" style={{ width: `${props.ratio * 100}%` }} />
        </div>
    </div>
);

export default Progress;
