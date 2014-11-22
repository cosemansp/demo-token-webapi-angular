(function() {
    'use strict';

    var events = {
        userLoggedIn: "userLoggedIn",
        userLoggedOut: "userLoggedOut"
    };

    angular.module('myApp', ['ngRoute'])
        .constant("events", events)
        .config(routeConfig)
        .run(initIdentity)
        .run(handleRouteChange);

    function handleRouteChange($rootScope, $location) {
        $rootScope.$on("$routeChangeStart", function (event, currRoute, prevRoute) {
            if (!$rootScope.identity.isAuthenticated()) {
                $location.path("login");
                return;
            }

            /*
            if (!$rootScope.indentity.isInAnyRole(currRoute.access)) {
                $rootScope.error = "Seems like you tried accessing a route you don't have access to...";
                theEvent.preventDefault();

                // do some error handling: no access
            }*/
        });
    }

    function initIdentity($rootScope, $log, authService) {
        $rootScope.identity = authService.init();
        $log.info("initIdentity: " + $rootScope.identity.name);
    };

    function routeConfig($routeProvider) {
        $routeProvider
            .when("/main", {
                    templateUrl: "main.html",
                    controller: "MainController",
                    controllerAs: "vm",
                    access: "admin"
                }
            )
            .when("/login", {
                    templateUrl: "login.html",
                    controller: "LoginController",
                    controllerAs: "vm",
                    access: ""
                }
            )
            .when("/editProfile", {
                    templateUrl: "editProfile.html",
                    controller: "EditProfileController",
                    controllerAs: "vm",
                    access: "admin"
                }
            )
            .otherwise({
                redirectTo: '/'
            });
    };

})();