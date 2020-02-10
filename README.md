# GitHubAutoUpdater

## GitHubAutoUpdater�ŉ����ł���̂��H
�����**Windows**�A�v���P�[�V�������o�[�W�����A�b�v���邽�߂́A�A�v���P�[�V�����ł��B

GitHub��̓���̃��|�W�g���̍ŐV�̃����[�X����ǂݎ��A�o�[�W�����̍��ق��`�F�b�N���āA�A�v���P�[�V�������_�E�����[�h���ăA�b�v�f�[�g�����{���܂��B

## �A�v���P�[�V�����̊ȒP�ȓ������
�ȒP�ȓ�������͈ȉ��̂Ƃ���ł��B

 1. GitHub��̓���̃��|�W�g���́h�ŐV�̃����[�X���h��ǂݎ��o�[�W���������擾���܂��B
 2. ���[�J���}�V���̃C���X�g�[���ς݂̃A�v���P�[�V�����̃o�[�W�������擾���܂��B
 3. ���҂̃o�[�W�������r���ĐV�������̂ł��邩���m�F���܂��B
 4. �o�[�W�������V������΁AGitHub��̍ŐV�����[�X����assets�t�@�C�����_�E�����[�h���܂��B
 5. �_�E�����[�h��ɁA�A�b�v�f�[�g�����{���܂��B

## GitHubAutoUpdater�̃t�@�C���\��
���|�W�g������[releases](https://github.com/ison12/gitHubAutoUpdater/releases)����A�v���P�[�V�����t�@�C���ꎮ���擾���Ă��������B

| �t�@�C���� | ���� |
|--|--|
| GitHubAutoUpdater.config | �A�b�v�f�[�g�̂��߂̐ݒ�t�@�C���i�v�ҏW�j |
| GitHubAutoUpdater.exe | ���s�t�@�C�� |
| GitHubAutoUpdater.exe.config | ���s�t�@�C���̐ݒ�t�@�C���i�ҏW�s�v�j |

## GitHubAutoUpdater.config�̐ݒ��
�d�v�Ȃ̂́AGitHubAutoUpdater.config�t�@�C���ł��B

�ݒ��Ƃ��āA[CliboDone](https://github.com/ison12/cliboDone)���A�b�v�f�[�g���邽�߂̓��e�������܂��B
�܂��̓t�H���_�\������ł��B

**�t�H���_�\��**

    CliboDone
    ��  CliboDone.config
    ��  CliboDone.exe
    ��  CliboDone.exe.config
    ��  CliboDone.version
    ��  Update.bat
    ��
    ����ConvertScripts
    ��
    ����GitHubAutoUpdater
    ��  ��  GitHubAutoUpdater.config
    ��  ��  GitHubAutoUpdater.exe
    ��  ��  GitHubAutoUpdater.exe.config
    ��
    ����Manual

�����ŏd�v�Ȃ̂́AGitHubAutoUpdater.exe��CliboDone.exe�̂��݂��̈ʒu�֌W�ł��B
�ݒ�t�@�C�����ȉ��̂悤�ɂ��܂��B

**�ݒ�t�@�C��**

    <?xml version="1.0" encoding="utf-8" ?>
    <root>
      <applicationName>CliboDone</applicationName>
      <applicationFilePath>..\CliboDone.exe</applicationFilePath> �c (1)
      <applicationVersionFilePath>..\CliboDone.version</applicationVersionFilePath> �c (2)
      <updateScriptFilePath>Update.bat</updateScriptFilePath> �c (3)
      <updateCheckProcesses>
        <process>CliboDone</process> �c (4)
      </updateCheckProcesses>
      <github>
        <rootUrl>https://api.github.com</rootUrl>
        <releasesLatest>
          <uri>repos/:owner/:repo/releases/latest</uri>
          <owner>ison12</owner> �c (5)
          <repo>cliboDone</repo> �c (6)
        </releasesLatest>
      </github>
    </root>

�d�v�ȓ_�́A��L�̐ݒ�t�@�C�����̔ԍ������ł��c **(1)�`(6)**

 1. GitHubAutoUpdater.exe���猩���A�ΏۃA�v���iCliboDone.exe�j�̑��΃p�X
 2. GitHubAutoUpdater.exe���猩���A�ΏۃA�v���̃o�[�W�����t�@�C���iCliboDone.version�j�̑��΃p�X
 3. �ΏۃA�v���iCliboDone.exe�j���猩���A�A�b�v�f�[�g�X�N���v�g�̑��΃p�X
 4. �A�b�v�f�[�g���s���Ƀ`�F�b�N���ׂ��v���Z�X�̈ꗗ�i�g���q�͕s�v�j
 5. �ΏۃA�v����GitHub���L�Җ�
 6. �ΏۃA�v����GitHub���|�W�g����

���̂悤�ɐݒ肵����ŁAGitHubAutoUpdater.exe�����s���邱�ƂŃA�v���P�[�V�����̃A�b�v�f�[�g���\�ɂȂ�܂��B

## GitHubAutoUpdater���g�������ꍇ�Ƀ��[�U�[���p�ӂ������

**GitHub�֘A**
 - GitHub�A�J�E���g
 - GitHub��Windows�A�v���P�[�V���������J���郊�|�W�g��
  - GitHub�̃����[�X�@�\��Windows�A�v���P�[�V�����������[�X���邱�ƁB�����[�X����assets�t�@�C���ɃA�v���P�[�V�����ꎮ��zip�t�@�C���Ŋ܂߂邱�ƁB

**Windows�A�v���P�[�V�����̓��e**
 - Windows�A�v���P�[�V�������A�b�v�f�[�g���邽�߂̃A�b�v�f�[�g�X�N���v�g��p�ӂ��邱�Ɓi�V�F�����s�ł���Ίg���q�͖��Ȃ��j�B
 - �A�b�v�f�[�g�X�N���v�g���ŉ����G���[�����������ꍇ�͏I���R�[�h��0�ȊO�ɂ��āA����̏ꍇ�͏I���R�[�h��0�ɂ��܂��B
 - Windows�A�v���P�[�V�����̃o�[�W���������i�[���邽�߂̃o�[�W�����t�@�C����p�ӂ��邱�ƁB������̃t�@�C���ɂ́A�A�b�v�f�[�g�����s����邽�тɁAGitHub�̃����[�X�^�O�̔ԍ����X�V�����B
 - Windows�A�v���P�[�V��������GitHubAutoUpdater.exe���Ăяo�����j���[�Ȃǂ��ݒu����Ă��邱�ƁB
