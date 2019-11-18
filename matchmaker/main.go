package main

import (
	"github.com/google/logger"
	"github.com/gorilla/mux"
	"github.com/urfave/negroni"
	"io/ioutil"
	"net/http"
)

const addr = "localhost:4444"

func main() {
	log := logger.Init("Logger", true, true, ioutil.Discard)
	defer log.Close()

	startServer()
}

func startServer() {
	router := mux.NewRouter()

	router.Handle("/list", http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		//json.NewEncoder(w).Encode(map[string]bool{"ok": true})
	})).Methods("GET")

	router.Handle("/start", http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		//json.NewEncoder(w).Encode(map[string]bool{"ok": true})
	})).Methods("PUT")

	router.Handle("/join/{id}", http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		//json.NewEncoder(w).Encode(map[string]bool{"ok": true})
	})).Methods("POST")

	negroniServer := negroni.Classic()
	negroniServer.UseHandler(router)

	logger.Infof("Starting server on %s", addr)

	server := &http.Server{
		Addr:    addr,
		Handler: negroniServer,
	}

	if err := server.ListenAndServe(); err != nil {
		logger.Fatalf("Failed to start server: %v", err)
	}
}
