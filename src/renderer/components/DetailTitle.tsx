import React from 'react';

import './DetailTitle.scss';

type DetailTitleProps = {
    onBackButtonClick?(e: React.MouseEvent<HTMLDivElement>): void;
};

const DetailTitle: React.FC<React.PropsWithChildren<DetailTitleProps>> = (props) => (
    <div className="detail-title">
        <div className="back-btn" onClick={props.onBackButtonClick}>
            <i className="ion ion-ios-close-circle-outline" />
        </div>

        <h2>{props.children}</h2>
    </div>
);

export default DetailTitle;
