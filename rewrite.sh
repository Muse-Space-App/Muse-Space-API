
git filter-branch -f --env-filter '
if [ "$GIT_COMMIT" = "3bc792dca8b06206cfecd3b40919983c0d56b5d1" ]; then
  export GIT_AUTHOR_DATE="$(date -d '14.0 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "c33dc683e0548bf0a4f740216cc23c5322b1fff1" ]; then
  export GIT_AUTHOR_DATE="$(date -d '13.64102564102564 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "0930086121abdcbfaaffe648d209e827abc273af" ]; then
  export GIT_AUTHOR_DATE="$(date -d '13.282051282051283 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "71e83d7559ec9d14313be8838541ebaa06d44b6c" ]; then
  export GIT_AUTHOR_DATE="$(date -d '12.923076923076923 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "40e1dfc87f1eec11864c35cce9c410fa7fa4622d" ]; then
  export GIT_AUTHOR_DATE="$(date -d '12.564102564102564 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "95bbbcbe0d931b3f5786c2554e0d40539d879ad0" ]; then
  export GIT_AUTHOR_DATE="$(date -d '12.205128205128204 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "7360e5a7c35d28e04e16a592b4cf296ba2626f48" ]; then
  export GIT_AUTHOR_DATE="$(date -d '11.846153846153847 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "ade8bbccfc29334bcf212ae798587a08f3ee6871" ]; then
  export GIT_AUTHOR_DATE="$(date -d '11.487179487179487 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "ada0cea6e9b8ff579574451f05f780e269982d93" ]; then
  export GIT_AUTHOR_DATE="$(date -d '11.128205128205128 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "b76902abb00ff1a582d4e97a92f36aab8cba2403" ]; then
  export GIT_AUTHOR_DATE="$(date -d '10.76923076923077 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "27a042ddd3a2bbe31681568b44610e79f95c6c7b" ]; then
  export GIT_AUTHOR_DATE="$(date -d '10.41025641025641 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "e8341c59b8f524cf54d57fc24fc9322252a2fe7b" ]; then
  export GIT_AUTHOR_DATE="$(date -d '10.051282051282051 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "76f2c788bf7c5f68e090b3feddd694a470a686c7" ]; then
  export GIT_AUTHOR_DATE="$(date -d '9.692307692307693 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "6bd78b43ada9c381e58920edc0691a3d92294aa5" ]; then
  export GIT_AUTHOR_DATE="$(date -d '9.333333333333332 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "411ec557c18dff199bd327c6c25002019443768f" ]; then
  export GIT_AUTHOR_DATE="$(date -d '8.974358974358974 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "bf7b9b0036d2e71532aab8b9cb89b14c46e87c92" ]; then
  export GIT_AUTHOR_DATE="$(date -d '8.615384615384615 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "29c267f3bed25d7688624a7f8f07641f54bfbec2" ]; then
  export GIT_AUTHOR_DATE="$(date -d '8.256410256410255 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "6af6fd1f3b8833173aa6294ad5dd0c05b8fabf79" ]; then
  export GIT_AUTHOR_DATE="$(date -d '7.897435897435898 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "5713b199c79e88159fc257087b6c17e89eeb0f2e" ]; then
  export GIT_AUTHOR_DATE="$(date -d '7.538461538461538 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "8292c371043fb3ad6b5e1ca2c11e29fce14f0281" ]; then
  export GIT_AUTHOR_DATE="$(date -d '7.17948717948718 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "48d9d760ebf69e5e29c7a39a640363d2dbd9a197" ]; then
  export GIT_AUTHOR_DATE="$(date -d '6.82051282051282 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "e1bf515b17d8e240212f1849b5ca7133ddcf6c26" ]; then
  export GIT_AUTHOR_DATE="$(date -d '6.461538461538462 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "c2ee0d306dbdf0ac9bf5fa2ed71d4f940850e085" ]; then
  export GIT_AUTHOR_DATE="$(date -d '6.102564102564102 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "3dff6bfaec5789e3f405dd44d0754618d8861c68" ]; then
  export GIT_AUTHOR_DATE="$(date -d '5.743589743589743 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "aa55061a90fe6f11003acc960b669ce3fa9ad2d5" ]; then
  export GIT_AUTHOR_DATE="$(date -d '5.384615384615385 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "fd1847124420dd7e1b4c10d010e4d602fab7f2c8" ]; then
  export GIT_AUTHOR_DATE="$(date -d '5.0256410256410255 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "5d575ca98717bbfc02a0842b8ce1945818e77365" ]; then
  export GIT_AUTHOR_DATE="$(date -d '4.666666666666666 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "427474fb4d909e04ce67809d83a85bd9c63fa084" ]; then
  export GIT_AUTHOR_DATE="$(date -d '4.307692307692308 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "b732d9706d956396e11da389deb96b9fe65beea4" ]; then
  export GIT_AUTHOR_DATE="$(date -d '3.948717948717949 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "fd3debd329c36db9b157c09c0a9659bbb9da71c7" ]; then
  export GIT_AUTHOR_DATE="$(date -d '3.5897435897435894 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "2fd3382ab5cb8e3c794889d7469bc4eb2a6da34b" ]; then
  export GIT_AUTHOR_DATE="$(date -d '3.23076923076923 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "4fc313dc1af8118ce587ffbee03a23c93f4fd22b" ]; then
  export GIT_AUTHOR_DATE="$(date -d '2.8717948717948723 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "1c339ff64895100d950cdded9c27af913e663c8f" ]; then
  export GIT_AUTHOR_DATE="$(date -d '2.5128205128205128 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "35ed7a091715ffa453ac55612cbd8ef689c5b49d" ]; then
  export GIT_AUTHOR_DATE="$(date -d '2.1538461538461533 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "b1d387e8b073e2ac73ef4be40830e87947e3bc0e" ]; then
  export GIT_AUTHOR_DATE="$(date -d '1.7948717948717956 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "752708a4651009f6f7b203c8384ee11e659305b4" ]; then
  export GIT_AUTHOR_DATE="$(date -d '1.4358974358974361 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "c3262c3a5a67f5ff10614ab57c270acd90fd8609" ]; then
  export GIT_AUTHOR_DATE="$(date -d '1.0769230769230766 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "af140bd5b184629ce3183c4c555b8a253615aea6" ]; then
  export GIT_AUTHOR_DATE="$(date -d '0.7179487179487172 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi
if [ "$GIT_COMMIT" = "de843285c95c5fb3fd4b03c781d765a1887aa679" ]; then
  export GIT_AUTHOR_DATE="$(date -d '0.3589743589743595 days ago')"
  export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"
fi

' -- --all
