// tripsController.js

(function () {
    "use strict";

    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {

        var vm = this;

        vm.trips = [];

        vm.newTrip = {};

        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips")
            .then(function(response) {
                //worky
                angular.copy(response.data, vm.trips);

            }, function(error) {
                //no worky
                vm.errorMessage = "Failed to load data: " + error;
            })
            .finally(function() {
                vm.isBusy = false;
            });

        vm.addTrip = function () {

            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/trips", vm.newTrip)
                .then(function(response) {
                    //worky
                    vm.trips.push(response.data);
                    vm.newTrip = {};
                }, function() {
                    //no worky
                    vm.errorMessage = "Failed to save new trip: " + error;
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        };



    }

})();