(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('pluginController', ['$scope', 'instancePluginsService',
            'notificationService', '$location', '$routeParams', '$timeout', 'communicationTypes', 'inputTypes', pluginController]);

    function pluginController($scope, instancePluginsService, notificationService,
        $location, $routeParams, $timeout, communicationTypes, inputTypes) {
        $scope.plugin = {};

        if (!($routeParams.id && $routeParams.version)) {
            $location.path('/plugins/');
        }

        $scope.loadPlugin = function () {
            instancePluginsService.loadPlugin($routeParams.id, $routeParams.version)
                .success(function (data) {
                    $scope.plugin = data;

                    $timeout(function () {
                        SyntaxHighlighter.highlight();
                    }, 0);

                }).error(function (err) {
                    console.log(err);
                    notificationService.addNotification('plugin', 'error while loading plugin info', 'error');
                });
        }

        $scope.roundPluginSize = function (size) {
            var number = new Number(size * 0.000001);
            return number.toFixed(2);
        }

        $scope.formatDate = function(date) {
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
