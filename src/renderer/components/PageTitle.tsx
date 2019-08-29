import React from 'react';

import './PageTitle.scss';

const PageTitle: React.FC<React.PropsWithChildren<{}>> = (props) => (
    <h1 className="page-title">
        {props.children}
    </h1>
);

export default PageTitle;
