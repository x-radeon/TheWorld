﻿// tripEditorController.js
(function() {
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams, $http) {
        var vm = this;
        
        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.newStop = {};
        vm.isBusy = true;
        var url = "/api/trips/" + vm.tripName + "/stops";

        $http.get(url)
            .then(function(response) {
                //success
                if (response.data.length == 0) {
                    vm.noStopsMessage = "There are no stops in this Trip!";
                } else {
                    angular.copy(response.data, vm.stops);
                    _showMap(vm.stops);
                }
            }, function (err) {
                //failure
                vm.errorMessage = "Failed to load stops";
            })
            .finally(function() {
                vm.isBusy = false;
            });

        vm.addStop = function() {
            vm.isBusy = true;

            $http.post(url, vm.newStop)
                .then(function(response) {
                    //success
                    vm.stops.push(response.data);
                    _showMap(vm.stops);
                    vm.newStop = {};
                }, function(err) {
                    //failure
                    vm.errorMessage = "Failed to add new stop";
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        };
    }

    function _showMap(stops) {
        
        if (stops && stops.length > 0) {
            // show map

            var mapStops = _.map(stops, function(item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                }
            });

            travelMap.createMap({
                stops: mapStops,
                selector: "#map",
                currentStop: 1,
                initialZoom: 3
            })
        }

    }

})();