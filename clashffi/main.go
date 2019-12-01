package main

/*
#include <stdlib.h>
*/
import "C"

import (
	"fmt"
	"os"
	"unsafe"

	"github.com/Dreamacro/clash/config"
	"github.com/Dreamacro/clash/constant"
	"github.com/Dreamacro/clash/hub"
	"github.com/oschwald/geoip2-golang"
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

//export clash_config_dir
func clash_config_dir() *C.char {
	return C.CString(constant.Path.HomeDir())
}

//export clash_mmdb_ok
func clash_mmdb_ok() bool {
	_, err := os.Stat(constant.Path.MMDB())
	if os.IsNotExist(err) {
		return false
	}

	_, err = geoip2.Open(constant.Path.MMDB())
	return err == nil
}

//export free_cstr
func free_cstr(str *C.char) {
	C.free(unsafe.Pointer(str))
}

func main() {}
