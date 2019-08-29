import React from 'react';

import './Switch.scss';

type SwitchProps = {
    on: boolean;
    onClick?(e: React.MouseEvent<HTMLDivElement>): void;
};

const Switch: React.FC<SwitchProps> = (props) => (
    <div className={`switch ${props.on ? 'on' : ''}`} onClick={props.onClick}>
        <div className="switch-inner" />
    </div>
);

export default Switch;
