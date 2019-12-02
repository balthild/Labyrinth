package main

/*
#include <stdlib.h>

typedef struct {
	char code;
	char* addr;
	char* secret;
} StartResult;
*/
import "C"

import (
	"fmt"
	"net"
	"os"
	"path/filepath"
	"unsafe"

	"github.com/Dreamacro/clash/constant"
	"github.com/Dreamacro/clash/hub/executor"
	"github.com/Dreamacro/clash/hub/route"
	"github.com/oschwald/geoip2-golang"
	"github.com/phayes/freeport"
)

//export clash_start
func clash_start() *C.StartResult {
	result := (*C.StartResult)(C.malloc(C.size_t(unsafe.Sizeof(C.StartResult{}))))
	result.code = 0
	result.addr = nil
	result.secret = nil

	constant.SetConfig(filepath.Join(constant.Path.HomeDir(), "config.yaml"))

	c, err := executor.Parse()
	if err != nil {
		fmt.Println(err.Error())
		result.code = 1
		return result
	}

	addr, err := net.ResolveTCPAddr("tcp", c.General.ExternalController)
	if err != nil {
		frp, err := freeport.GetFreePort();
		if err != nil {
			result.code = 2
			return result
		}
		c.General.ExternalController = "localhost:" + string(frp)
	} else {
		listener, err := net.ListenTCP("tcp", addr)
		if err != nil {
			frp, err := freeport.GetFreePort()
			if err != nil {
				result.code = 2
				return result
			}
			c.General.ExternalController = "localhost:" + string(frp)
		} else {
			listener.Close()
		}
	}

	go route.Start(c.General.ExternalController, c.General.Secret)

	executor.ApplyConfig(c, true)

	result.addr = C.CString(c.General.ExternalController)
	result.secret = C.CString(c.General.Secret)
	return result
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

//export c_free
func c_free(ptr uintptr) {
	C.free(unsafe.Pointer(ptr))
}

func main() {}
