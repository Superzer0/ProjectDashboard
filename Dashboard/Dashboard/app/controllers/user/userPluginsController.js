﻿(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('userPluginsController', ['$scope', 'notificationService',
            'utilsService', 'instancePluginsService',
            userPluginsController]);

    function userPluginsController($scope, notificationService, utilsService, instancePluginsService) {

        $scope.plugins = null;

        $scope.loadUserPlugins = function () {
            instancePluginsService.loadUserPlugins()
                .success(function (data) {
                    if (!angular.equals([], data)) {
                        $scope.plugins = data;
                    }
                }).error(function (err) {
                    notificationService.addNotification('user plugins', 'error while loading user plugins list', 'error');
                    console.log(err);
                });
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.loadUserPlugins();
    }

    angular
        .module('angularAdmin')
        .controller('userPluginConfigurationController', ['$scope', 'notificationService',
            'utilsService', 'instancePluginsService', '$routeParams', '$timeout', '$location', userPluginConfigurationController]);

    function userPluginConfigurationController($scope, notificationService, utilsService,
        instancePluginsService, $routeParams, $timeout, $location) {

        $scope.plugin = null;

        if (!($routeParams.id && $routeParams.version)) {
            $location.path('/user/index');
        }

        $scope.loadPlugin = function () {
            instancePluginsService.loadUserPluginConfiguration($routeParams.id, $routeParams.version)
                .success(function (data) {
                    if (!angular.equals([], data)) {
                        $scope.plugin = data;
                        $timeout(function () {
                            SyntaxHighlighter.highlight();
                        }, 0);
                    }

                }).error(function (err) {
                    notificationService.addNotification('active plugins', 'error while loading active plugins list', 'error');
                    console.log(err);
                });
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }
        $scope.loadPlugin();
    }

})();
