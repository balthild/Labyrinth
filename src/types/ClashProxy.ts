export type History = {
    time: string;
    delay: number;
};

export type Proxy = {
    history: History[];
    type: string;
};

export type Group = {
    all: string[];
    now: string;
    type: string;
};

export type Direct = {
    type: 'Direct';
};

export type Reject = {
    type: 'Reject';
};

export type URLTest = Group & {
    type: 'URLTest';
};

export type Fallback = Group & {
    type: 'Fallback';
};

export type LoadBalance = Group & {
    type: 'LoadBalance';
};

export type Selector = Group & {
    type: 'Selector';
};

export type Shadowsocks = Proxy & {
    type: 'Shadowsocks';
};

export type Vmess = Proxy & {
    type: 'Vmess';
};

export type Http = Proxy & {
    type: 'Http';
};

export type Socks5 = Proxy & {
    type: 'Socks5';
};

export type ProxyItem =
    | Direct
    | Reject
    | URLTest
    | Fallback
    | LoadBalance
    | Selector
    | Shadowsocks
    | Vmess
    | Http
    | Socks5
    ;

export type Proxies = Record<string, ProxyItem>;
