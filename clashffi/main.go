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
	"github.com/phayes/freeport"
)

//export clash_start
func clash_start() (C.int, *C.char, *C.char) {
	constant.SetConfig(filepath.Join(constant.Path.HomeDir(), "config.yaml"))

	c, err := executor.Parse()
	if err != nil {
		fmt.Println(err.Error())
		return 1, nil, nil
	}

	addr, err := net.ResolveTCPAddr("tcp", c.General.ExternalController)
	if err != nil {
		frp, err := freeport.GetFreePort();
		if err != nil {
			return 2, nil, nil
		}
		c.General.ExternalController = "localhost:" + string(frp)
	} else {
		listener, err := net.ListenTCP("tcp", addr)
		if err != nil {
			frp, err := freeport.GetFreePort()
			if err != nil {
				return 2, nil, nil
			}
			c.General.ExternalController = "localhost:" + string(frp)
		} else {
			listener.Close()
		}
	}

	go route.Start(c.General.ExternalController, c.General.Secret)

	executor.ApplyConfig(c, true)

	return 0, C.CString(c.General.ExternalController), C.CString(c.General.Secret)
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

//export c_free
func c_free(ptr unsafe.Pointer) {
	C.free(ptr)
}

func main() {}
