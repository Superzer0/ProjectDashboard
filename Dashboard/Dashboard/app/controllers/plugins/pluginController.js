(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('pluginController', ['$scope', 'instancePluginsService',
            'notificationService', '$location', '$routeParams', '$timeout', 'communicationTypes',
            'inputTypes', '$http', pluginController]);

    function pluginController($scope, instancePluginsService, notificationService,
        $location, $routeParams, $timeout, communicationTypes, inputTypes, $http) {
        $scope.plugin = {};

        if (!($routeParams.id && $routeParams.version)) {
            $location.path('/plugins/');
        }

        $scope.loadPlugin = function () {
            instancePluginsService.loadPlugin($routeParams.id, $routeParams.version)
                .success(function (data) {
                    $scope.plugin = data;
                    $scope.pluginState = !data.disabled;
                    $timeout(function () {
                        SyntaxHighlighter.highlight();
                    }, 0);

                }).error(function (err) {
                    console.log(err);
                    notificationService.addNotification('plugin', 'error while loading plugin info', 'error');
                });
        }

        $scope.callPlugin = function () {
            $http.post('api/dispatch/' + $scope.plugin.id + '/' + $scope.plugin.version + '/GetData',
                '{configuration:"heheh"}')
                .success(function (response) {
                    console.log(response);
                }).error(function (err) {
                    console.error(err);
                });
        };

        $scope.onPluginStateChange = function (state) {
            var valueToChange = !state;
            instancePluginsService.setPluginStateForInstance($scope.plugin.id, $scope.plugin.version, valueToChange)
                .success(function () {
                    $scope.plugin.disabled = valueToChange;
                }).error(function () {
                    notificationService.addNotification('plugin state', 'error while changing state for ' + $scope.plugin.name, 'error');
                    $scope.pluginState = !$scope.plugin.disabled;
                });
        };

        $scope.roundPluginSize = function (size) {
            var number = new Number(size * 0.000001);
            return number.toFixed(2);
        }

        $scope.formatDate = function (date) {
            return moment(date, "YYYYMMDD");
        }

        $scope.getCommunicationTypeLabel = function (index) {
            return communicationTypes[index];
        }

        $scope.getInputTypeLabel = function (index) {
            return inputTypes[index];
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.loadPlugin();
    }
})();
