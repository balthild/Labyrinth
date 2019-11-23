package main

import (
	"C"
	"fmt"

	"github.com/Dreamacro/clash/config"
	"github.com/Dreamacro/clash/constant"
	"github.com/Dreamacro/clash/hub"
)

//export clash_start
func clash_start() int8 {
	if err := config.Init(constant.Path.HomeDir()); err != nil {
		fmt.Println("Initial configuration directory error:", err.Error())
		return 1
	}

	if err := hub.Parse(); err != nil {
		fmt.Println("Parse config error:", err.Error())
		return 2
	}

	return 0
}

func main() {}
