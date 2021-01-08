package main

/*
#include <stdlib.h>
*/
import "C"

import (
	"fmt"
	"net"
	"os"
	"path/filepath"
	"unsafe"

	"github.com/Dreamacro/clash/config"
	"github.com/Dreamacro/clash/constant"
	"github.com/Dreamacro/clash/hub/executor"
	"github.com/Dreamacro/clash/hub/route"
	"github.com/oschwald/geoip2-golang"
)

const (
	FailedToParseConfig       = 1
	InvalidControllerAddr     = 2
	ControllerPortUnavailable = 3
)

//export clash_start
func clash_start() (C.int, *C.char, *C.char) {
	constant.SetConfig(filepath.Join(constant.Path.HomeDir(), "config.yaml"))

	// Parse the config file
	cfg, err := executor.Parse()
	if err != nil {
		fmt.Println(err.Error())
		return FailedToParseConfig, nil, nil
	}

	// Check if the address is valid
	addr, err := net.ResolveTCPAddr("tcp", cfg.General.ExternalController)
	if err != nil {
		fmt.Println(err.Error())
		return InvalidControllerAddr, nil, nil
	}

	// Check if the address is available
	listener, err := net.ListenTCP("tcp", addr)
	if err != nil {
		fmt.Println(err.Error())
		return ControllerPortUnavailable, nil, nil
	} else {
		listener.Close()
	}

	// Start the controller
	go route.Start(cfg.General.ExternalController, cfg.General.Secret)

	// Start the proxy
	executor.ApplyConfig(cfg, true)

	return 0, C.CString(cfg.General.ExternalController), C.CString(cfg.General.Secret)
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

//export clash_validate_config
func clash_validate_config(ptr unsafe.Pointer, len C.int) *C.char {
	data := C.GoBytes(ptr, len)
	_, err := config.Parse(data)

	if err != nil {
		return C.CString(err.Error())
	}
	return nil
}

//export clash_version
func clash_version() *C.char {
	// TODO: automatically generating version string from go.mod?
	return C.CString("v1.3.5")
}

//export c_free
func c_free(ptr unsafe.Pointer) {
	C.free(ptr)
}

func main() {}
