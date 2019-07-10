import React, { PropsWithChildren } from 'react';

import './OverlayMessage.scss';

type OverlayMessageProps = {
    show: boolean;
    title?: string;
    onDismiss?(): void;
};

const OverlayMessage: React.FunctionComponent<PropsWithChildren<OverlayMessageProps>> = (props) => (
    <div className={'overlay' + (props.show ? ' show' : '')}>
        <div className="shade" onClick={props.onDismiss} />
        <div className="message-box">
            {props.title && (
                <h3 className="message-title">{props.title}</h3>
            )}
            {props.children}
        </div>
    </div>
);

export default OverlayMessage;
