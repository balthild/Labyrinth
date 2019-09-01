import React from 'react';

import './Detail.scss';

type DetailTitleProps = {
    isOpened: boolean;
    title: string;
    onCloseButtonClick?(e: React.MouseEvent<HTMLDivElement>): void;
};

const Detail: React.FC<React.PropsWithChildren<DetailTitleProps>> = (props) => (
    <div className={`detail-overlay ${props.isOpened ? 'open' : ''}`}>
        <div className="detail-shade" />
        <div className="detail content-inner">
            <div className="detail-title">
                <div className="close-btn" onClick={props.onCloseButtonClick}>
                    <i className="ion ion-ios-close-circle-outline" />
                </div>

                <h2>{props.title}</h2>
            </div>

            {props.children}
        </div>
    </div>
);

export default Detail;
