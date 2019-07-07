const target = process.env.MIX_TARGET;

if (target !== 'renderer' && target !== 'main') {
  console.error('MIX_TARGET must be renderer or main');
  process.exit(-1);
}

require(`./webpack.mix.${target}`);
